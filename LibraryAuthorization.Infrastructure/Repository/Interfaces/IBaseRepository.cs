using System.Linq.Expressions;

namespace LibraryAuthorization.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task AddAsync(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAndAddToContextAsync(Expression<Func<T, bool>> predicate);
}
