using CollegeSystemApi.Data;
using CollegeSystemApi.DTOs;
using CollegeSystemApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CollegeSystemApi.Services;

public class GenericServices<T> : IGenericServices<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericServices(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<ResponseDto<T>> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new ResponseDto<T>
        {
            Data = entity,
            StatusCode = 200,
            Message = "Success"
        };
    }

    public virtual async Task<ResponseDto<T>> Delete(T entity)
    {
        _context.Entry(entity).State = EntityState.Deleted;
        await _context.SaveChangesAsync();

        return new ResponseDto<T>
        {
            StatusCode = 200,
            Message = "Deleted successfully"
        };
    }

    public virtual async Task<ResponseDto<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var results = await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        if (results == null || !results.Any())
            return new ResponseDto<IEnumerable<T>>
            {
                StatusCode = 404,
                Message = "Data not found"
            };

        return new ResponseDto<IEnumerable<T>>
        {
            Data = results,
            StatusCode = 200,
            Message = "Success"
        };
    }

    public virtual async Task<ResponseDto<IEnumerable<T>>> GetAllAsync()
    {
        var results = await _dbSet.AsNoTracking().ToListAsync();
        return new ResponseDto<IEnumerable<T>>
        {
            Data = results,
            Message = "Success",
            StatusCode = 200
        };
    }

    public virtual async Task<ResponseDto<T>> GetByIdAsync(int id)
    {
        var result = await _dbSet.FindAsync(id);
        if (result == null)
            return new ResponseDto<T>
            {
                StatusCode = 404,
                Message = "Data not found"
            };

        return new ResponseDto<T>
        {
            Data = result,
            StatusCode = 200,
            Message = "Success"
        };
    }

    public virtual async Task<ResponseDto<T>> Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return new ResponseDto<T>
        {
            StatusCode = 200,
            Message = "Updated Successfully!"
        };
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}