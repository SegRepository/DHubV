using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Dal.Repository.GenricRepository
{
    public interface IGenericRepository<T, Y>
    where T : class
    {
        IQueryable<T> AsNoTracking();
        T GetById(Y id);
        IEnumerable<T> GetAll();
        IQueryable<T> FindCondition(
            Expression<Func<T, bool>> expression,
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] includes
        );
        IEnumerable<T> Find(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int page = 1,
            int pageSize = int.MaxValue,
            params Expression<Func<T, object>>[] includes
        );
        T Add(T entity);
        Task<T> DeleteAsync(T entity, bool permenant = false);
        Task<ICollection<T>> DeleteRangeAsync(ICollection<T> entries, bool permenant = false);
        T Delete(T entry, bool permenant = false);
        Task<T> AddAsync(T entity);
        T Edit(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(Y id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update();
        int Count();
        Task<IDbContextTransaction> BeginTransAsync(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified
        );
        IDbContextTransaction BeginTrans(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified
        );
        Task CommitAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task RollbackAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        Task SaveChangesAsync();
    }

}
