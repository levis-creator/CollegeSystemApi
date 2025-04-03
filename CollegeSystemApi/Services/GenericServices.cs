using System.Linq.Expressions;
using System.Net;
using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs.Response;
using CollegeSystemApi.Models.Common;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeSystemApi.Services
{
    /// <summary>
    /// Generic service class for performing CRUD operations on entities.
    /// </summary>
    public class GenericServices<T> : IGenericServices<T> where T : class, IEntity
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
        public virtual async Task<ResponseDtoData<T>> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ResponseDtoData<T>.SuccessResult(entity, "Successfully added.");
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
        public virtual async Task<ResponseDtoData<List<T>>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var results = await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
            if (!results.Any())
                return ResponseDtoData<List<T>>.ErrorResult(404, "Data not found");

            return ResponseDtoData<List<T>>.SuccessResult(results, "Data found.");
        }

        /// <summary>
        /// Gets all entities. 
        /// </summary>
        public virtual async Task<ResponseDtoData<List<T>>> GetAllAsync()
        {
            var results = await _dbSet.AsNoTracking().ToListAsync();
            return ResponseDtoData<List<T>>.SuccessResult(results, "Data retrieved successfully.");
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        public virtual async Task<ResponseDtoData<T>> GetByIdAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return result == null ? ResponseDtoData<T>.ErrorResult(404, "Item not found.") : ResponseDtoData<T>.SuccessResult(result, "Item found.");
        }

        /// <summary>
        /// Updates an entity and returns a success response.
        /// </summary>
        public virtual async Task<ResponseDtoData<T>> Update(int id, T entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    return ResponseDtoData<T>.ErrorResult((int)HttpStatusCode.NotFound, "Entity not found.");
                }

                // Ensure the entity being updated has the correct ID
                entity.Id = existingEntity.Id;

                // Copy all properties EXCEPT the key property
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                // Ensure the primary key is not modified
                _context.Entry(existingEntity).Property(e => e.Id).IsModified = false;

                await _context.SaveChangesAsync();
                return ResponseDtoData<T>.SuccessResult(existingEntity, "Successfully updated.");
            }
            catch (Exception ex)
            {
                return ResponseDtoData<T>.ErrorResult((int)HttpStatusCode.InternalServerError, "Update failed: " + ex.Message);
            }
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
