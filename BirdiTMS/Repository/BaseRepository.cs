using BirdiTMS.Context;
using BirdiTMS.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BirdiTMS.Repository
{
    public class BaseRepository<T>:IBaseRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context) {
        _context = context;
        }

        public async Task Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update( T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }


        public IQueryable<T> GetByQuery(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

    }
}
