using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null);
        Task<T> Get(Expression<Func<T, bool>> filter,
        string? includeProperties = null);
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task  RemoveRangeAsync(IEnumerable<T> entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter); 
    }
}
