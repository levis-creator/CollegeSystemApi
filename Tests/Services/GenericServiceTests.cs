using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.Services;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CollegeSystemApi.Tests.Services
{
    public abstract class GenericServiceTests<T> where T : class, new()
    {
        protected readonly Mock<ApplicationDbContext> _mockContext;
        protected readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        protected readonly IGenericServices<T> _service;
        protected Mock<DbSet<T>> MockDbSet { get; private set; }

        protected GenericServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            MockDbSet = new Mock<DbSet<T>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            // Setup default HTTP context
            var httpContext = new DefaultHttpContext();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _mockContext.Setup(c => c.Set<T>()).Returns(MockDbSet.Object);
            _service = new GenericServices<T>(_mockContext.Object, _mockHttpContextAccessor.Object);
        }

        #region Helper Methods
        protected void SetupMockDbSet(IEnumerable<T> data)
        {
            var queryable = data.AsQueryable();

            MockDbSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            MockDbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

            MockDbSet.As<IQueryable<T>>()
                .Setup(m => m.Expression).Returns(queryable.Expression);

            MockDbSet.As<IQueryable<T>>()
                .Setup(m => m.ElementType).Returns(queryable.ElementType);

            MockDbSet.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup for FindAsync
            MockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) =>
                    data.FirstOrDefault(e => (int)e.GetType().GetProperty("Id").GetValue(e) == (int)ids[0]));
        }

        protected void SetupEmptyMockDbSet()
        {
            SetupMockDbSet(new List<T>());
        }
        #endregion

        #region Test Classes
        protected class TestAsyncEnumerator<TEntity>(IEnumerator<TEntity> inner) : IAsyncEnumerator<TEntity>
        {
            public TEntity Current => inner.Current;
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
            public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(inner.MoveNext());
        }

        protected class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
        {
            public IQueryable CreateQuery(Expression expression) =>
                new TestAsyncEnumerable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
                new TestAsyncEnumerable<TElement>(expression);

            public object Execute(Expression expression) =>
                inner.Execute(expression) ?? throw new InvalidOperationException();

            public TResult Execute<TResult>(Expression expression) =>
                inner.Execute<TResult>(expression);

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                var resultType = typeof(TResult).GetGenericArguments()[0];
                var executionResult = typeof(IQueryProvider)
                    .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })?
                    .MakeGenericMethod(resultType)
                    .Invoke(this, new[] { expression });

                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))?
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { executionResult });
            }
        }

        protected class TestAsyncEnumerable<TEntity> : EnumerableQuery<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>
        {
            public TestAsyncEnumerable(IEnumerable<TEntity> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
                new TestAsyncEnumerator<TEntity>(this.AsEnumerable().GetEnumerator());
        }
        #endregion
    }

    public class TestEntityServiceTests : GenericServiceTests<TestEntity>
    {
        public TestEntityServiceTests() : base() { }

        [Fact]
        public async Task AddAsync_ShouldAddEntityAndReturnSuccessResponse()
        {
            // Arrange
            var entity = new TestEntity { Id = 1 };
            MockDbSet.Setup(d => d.AddAsync(entity, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityEntry<TestEntity>)null);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(entity);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successfully added.", result.Message);
            Assert.Equal(entity, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntityWhenFound()
        {
            // Arrange
            var testData = new List<TestEntity> { new TestEntity { Id = 1 } };
            SetupMockDbSet(testData);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFoundWhenEntityMissing()
        {
            // Arrange
            SetupEmptyMockDbSet();

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
    }
}