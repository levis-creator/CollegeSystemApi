using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.DTOs.Student;
using CollegeSystemApi.Models;
using CollegeSystemApi.Services.StudentServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace CollegeSystemApi.Tests.Services
{
    public class StudentCrudServiceTests
    {
        private readonly Mock<UserManager<Student>> _mockUserManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly StudentCrudService _service;

        public StudentCrudServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentTestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            // Mock UserManager
            var store = new Mock<IUserStore<Student>>();
            _mockUserManager = new Mock<UserManager<Student>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Initialize service
            _service = new StudentCrudService(_mockUserManager.Object, _dbContext);
        }

        [Fact]
        public async Task CreateStudentAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var testDept = new Department { Id = 1, DepartmentName = "Computer Science" };
            _dbContext.Departments.Add(testDept);
            await _dbContext.SaveChangesAsync();

            var newStudent = new StudentCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                NationalId = "12345678",
                AdmNo = "ST001",
                DepartmentId = 1
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<Student>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<Student>(), "Student"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Student)null);

            // Act
            var result = await _service.CreateStudentAsync(newStudent);

            // Assert
            Assert.True(result.Success);
       
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
        }

        [Fact]
        public async Task CreateStudentAsync_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var existingStudent = new Student { Email = "exists@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingStudent);

            var newStudent = new StudentCreateDto
            {
                Email = "exists@example.com",
                NationalId = "12345678"
            };

            // Act
            var result = await _service.CreateStudentAsync(newStudent);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.Conflict, result.StatusCode);
            Assert.Contains("already exists", result.Message);
        }

        [Fact]
        public async Task CreateStudentAsync_InvalidDepartment_ReturnsBadRequest()
        {
            // Arrange
            var newStudent = new StudentCreateDto { DepartmentId = 999 };

            // Act
            var result = await _service.CreateStudentAsync(newStudent);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Contains("department does not exist", result.Message);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ExistingId_ReturnsStudent()
        {
            // Arrange
            var testStudent = new Student
            {
                Id = "1",
                FirstName = "Test",
                LastName = "Student",
                Email = "test@example.com"
            };
            _dbContext.Students.Add(testStudent);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetStudentByIdAsync("1");

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetStudentByIdAsync_NonExistingId_ReturnsNotFound()
        {
            // Act
            var result = await _service.GetStudentByIdAsync("999");

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = "1", FirstName = "John" },
                new Student { Id = "2", FirstName = "Jane" }
            };
            _dbContext.Students.AddRange(students);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GetAllStudentsAsync();

            // Assert
            Assert.True(result.Success);
      
        }

        [Fact]
        public async Task UpdateStudentAsync_ValidData_ReturnsUpdatedStudent()
        {
            // Arrange
            var testStudent = new Student
            {
                Id = "1",
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                DepartmentId = 1
            };
            _dbContext.Students.Add(testStudent);
            await _dbContext.SaveChangesAsync();

            var updateDto = new StudentUpdateDto
            {
                FirstName = "New",
                LastName = "Name",
                Email = "new@example.com",
                NationalId = 12345678,
                DepartmentId = 2
            };

            // Add test department
            _dbContext.Departments.Add(new Department { Id = 2 });
            await _dbContext.SaveChangesAsync();

            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<Student>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.UpdateStudentAsync("1", updateDto);

            // Assert
       
        }

        [Fact]
        public async Task DeleteStudentAsync_ExistingId_ReturnsSuccess()
        {
            // Arrange
            var testStudent = new Student { Id = "1" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(testStudent);
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<Student>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.DeleteStudentAsync("1");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Student deleted successfully", result.Message);
        }

        [Fact]
        public async Task DeleteStudentAsync_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((Student)null);

            // Act
            var result = await _service.DeleteStudentAsync("999");

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateStudentAsync_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new StudentUpdateDto();

            // Act
            var result = await _service.UpdateStudentAsync("999", updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task UpdateStudentAsync_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            var existingStudent = new Student { Id = "1", Email = "old@example.com" };
            var otherStudent = new Student { Id = "2", Email = "exists@example.com" };
            _dbContext.Students.AddRange(existingStudent, otherStudent);
            await _dbContext.SaveChangesAsync();

            var updateDto = new StudentUpdateDto
            {
                Email = "exists@example.com"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(otherStudent);

            // Act
            var result = await _service.UpdateStudentAsync("1", updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal((int)HttpStatusCode.Conflict, result.StatusCode);
        }
    }
}