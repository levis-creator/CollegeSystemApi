using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CollegeSystemApi.Services
{
    public class GenericServices<T> : IGenericServices<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericServices(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Add a new entity and return a ResponseDto<T> with success message
        public virtual async Task<ResponseDto<T>> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ResponseDto<T>.SuccessResultForData(entity, "Successfully added.");
        }

        // Delete an entity and return a ResponseDto<T> indicating success or failure
        public virtual async Task<ResponseDto> Delete(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return ResponseDto.SuccessResult(null, "Successfully deleted.");
        }

        // Find entities based on a predicate and return a ResponseDto<List<T>> 
        public virtual async Task<ResponseDto<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var results = await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
            if (!results.Any())
                return ResponseDto<T>.ErrorResultForList(404, "Data not found", results);

            return ResponseDto<T>.SuccessResultForList(results, "Data found.");
        }

        // Get all entities and return a ResponseDto<List<T>> 
        public virtual async Task<ResponseDto<T>> GetAllAsync()
        {
            var results = await _dbSet.AsNoTracking().ToListAsync();
            return ResponseDto<T>.SuccessResultForList(results, "Data retrieved successfully.");
        }

        // Get an entity by its ID and return a ResponseDto<T> 
        public virtual async Task<ResponseDto<T>> GetByIdAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            if (result == null)
                return ResponseDto<T>.ErrorResultForData(404, "Item not found.", null);

            return ResponseDto<T>.SuccessResultForData(result, "Item found.");
        }

        // Update an entity and return a ResponseDto<T> indicating success or failure
        public virtual async Task<ResponseDto<T>> Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ResponseDto<T>.SuccessResultForData(entity, "Successfully updated.");
        }

        // Save changes and return the number of affected rows
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
