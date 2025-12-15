using LibraryAuthorization.Infrastructure.Data;
using LibraryAuthorization.Infrastructure.Repositories.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace LibraryAuthorization.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AuthDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AuthDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
