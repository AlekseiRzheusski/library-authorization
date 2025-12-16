namespace LibraryAuthorization.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task AddAsync(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task SaveAsync();
}
