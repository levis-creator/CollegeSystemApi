using CollegeSystemApi.DTOs.Response;
using System.Linq.Expressions;

namespace CollegeSystemApi.Services.Interfaces
{
    public interface IGenericServices<T> where T : class
    {
        Task<ResponseDtoData<T>> GetByIdAsync(int id);
        Task<ResponseDtoData<List<T>>> GetAllAsync();
        Task<ResponseDtoData<List<T>>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<ResponseDtoData<T>> AddAsync(T entity);
        Task<ResponseDtoData<T>> Update(int id, T entity);
        Task<ResponseDto> Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
