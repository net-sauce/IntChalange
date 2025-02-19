using System.Linq.Expressions;

namespace COMMON.REPO.Abstraction
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllMatches(Expression<Func<T, bool>> predicate);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate);
        Task<bool> Insert(T newState);
        Task<bool> Update(T oldValue,T newValue);
    }
}
