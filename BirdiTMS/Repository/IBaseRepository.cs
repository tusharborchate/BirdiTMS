using BirdiTMS.Models.Entities;
using System.Linq.Expressions;

namespace BirdiTMS.Repository
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetByQuery(Expression<Func<T, bool>> predicate);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
