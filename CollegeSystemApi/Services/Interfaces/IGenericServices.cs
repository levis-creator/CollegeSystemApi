using CollegeSystemApi.DTOs.Response;
using System.Linq.Expressions;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IGenericServices<T> where T : class
    {
        Task<ResponseDto<T>> GetByIdAsync(int id);
        Task<ResponseDto<T>> GetAllAsync();
        Task<ResponseDto<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<ResponseDto<T>> AddAsync(T entity);
        Task<ResponseDto<T>> Update(T entity);
        Task<ResponseDto> Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
