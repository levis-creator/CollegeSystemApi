using CollegeSystemApi.Data;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CollegeSystemApi.DTOs.Response;

namespace CollegeSystemApi.Services
{
    /// <summary>
    /// Generic service class for performing CRUD operations on entities.
    /// </summary>
    public class GenericServices<T> : IGenericServices<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the GenericServices class.
        /// </summary>
        public GenericServices(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Adds a new entity and returns a success response.
        /// </summary>
        public virtual async Task<ResponseDto<T>> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ResponseDto<T>.SuccessResultForData(entity, "Successfully added.");
        }

        /// <summary>
        /// Deletes an entity and returns a success response.
        /// </summary>
        public virtual async Task<ResponseDto> Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return ResponseDto.SuccessResult(null, "Successfully deleted.");
        }

        /// <summary>
        /// Finds entities based on a predicate.
        /// </summary>
        public virtual async Task<ResponseDto<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var results = await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
            if (!results.Any())
                return ResponseDto<T>.ErrorResultForData(404, "Data not found", null);

            return ResponseDto<T>.SuccessResultForList(results, "Data found.");
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        public virtual async Task<ResponseDto<T>> GetAllAsync()
        {
            var results = await _dbSet.AsNoTracking().ToListAsync();
            return ResponseDto<T>.SuccessResultForList(results, "Data retrieved successfully.");
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        public virtual async Task<ResponseDto<T>> GetByIdAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            if (result == null)
                return ResponseDto<T>.ErrorResultForData(404, "Item not found.", null);

            return ResponseDto<T>.SuccessResultForData(result, "Item found.");
        }

        /// <summary>
        /// Updates an entity and returns a success response.
        /// </summary>
        public virtual async Task<ResponseDto<T>> Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ResponseDto<T>.SuccessResultForData(entity, "Successfully updated.");
        }

        /// <summary>
        /// Saves changes asynchronously and returns the number of affected rows.
        /// </summary>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
