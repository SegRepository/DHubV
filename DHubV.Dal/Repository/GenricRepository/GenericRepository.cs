using DHubV.Core;
using DHubV.Dal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Dal.Repository.GenricRepository
{
    public class GenericRepository<T, Y> : IGenericRepository<T, Y>
         where T : class
    {
        protected readonly DVHubDBContext _context;
        internal DbSet<T> dbSet;
        public IDbContextTransaction Transaction { get; private set; }
        private bool asNoTracking = false;

        public GenericRepository(DVHubDBContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
            asNoTracking = false;
        }

        public IQueryable<T> All()
        {
            if (asNoTracking)
                return dbSet;
            else
                return dbSet.AsNoTracking();
        }

        public IQueryable<T> AsNoTracking()
        {
            asNoTracking = true;
            return dbSet.AsNoTracking(); // this.AsNoTracking();
        }

        //Add and save To database
        public T Add(T entity)
        {
            dbSet.Add(entity);
            return entity;
            //_context.SaveChanges();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
            //_context.SaveChanges();
        }

        public async Task<T> DeleteAsync(T entry, bool permenant = false)
        {
            if (entry is BaseGeneralModel baseEntry && !permenant)
            {
                baseEntry.IsDeleted = true;

                var navigationProperties = dbSet.Entry(entry).Metadata.GetNavigations();
                foreach (var navigationProperty in navigationProperties)
                {
                    if (navigationProperty.IsCollection)
                    {
                        var collection = dbSet.Entry(entry).Collection(navigationProperty.Name);
                        var collectionEntries = collection.CurrentValue as IEnumerable;

                        if (collectionEntries != null)
                        {
                            foreach (var item in collectionEntries)
                            {
                                if (item is BaseGeneralModel nestedEntry)
                                {
                                    nestedEntry.IsDeleted = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                dbSet.Entry(entry).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<ICollection<T>> DeleteRangeAsync(
            ICollection<T> entries,
            bool permenant = false
        )
        {
            foreach (var entry in entries)
            {
                await DeleteAsync(entry, permenant);
            }

            return entries;
        }

        public T Delete(T entry, bool permenant = false)
        {
            if (entry is BaseGeneralModel && !permenant)
            {
                foreach (
                    var property in dbSet
                        .Entry(entry)
                        .OriginalValues.Properties.Where(p => p.Name.Equals("IsDeleted"))
                )
                {
                    var entity = entry as BaseGeneralModel;
                    entity.IsDeleted = true;
                    dbSet.Entry(entry).Property(property).IsModified = true;
                }
            }
            else
            {
                dbSet.Entry(entry).State = EntityState.Deleted;
            }
            return entry;
        }

        public T Edit(T entity)
        {
            //_context.Entry(entity).State = EntityState.Modified;
            if (!asNoTracking)
                dbSet.Update(entity); //_context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);

            //_context.SaveChanges();
        }

        public int Count()
        {
            return dbSet.Count();
        }

        public IQueryable<T> FindCondition(
            Expression<Func<T, bool>> expression,
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] includes
        )
        {
            var query = dbSet.AsQueryable();
            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            if (includes != null)
                foreach (Expression<Func<T, object>> include in includes)
                    query = query.Include(include);

            if (expression != null)
            {
                query = query.Where(expression);
            }

            return query;
        }

        public IEnumerable<T> Find(
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int page = 1,
            int pageSize = int.MaxValue,
            params Expression<Func<T, object>>[] includes
        )
        {
            var query = dbSet.AsQueryable();
            if (includes != null)
                foreach (Expression<Func<T, object>> include in includes)
                    query = query.Include(include);

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else { }

            {
                /*
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    if (isSortAscending)
                    {
                        var propertyInfo = typeof(T).GetProperty(sortColumn);
                        if (propertyInfo != null)
                        {
                            var param = Expression.Parameter(typeof(T));
                            var expr = Expression.Lambda<Func<T, object>>(
                                Expression.Convert(Expression.Property(param, propertyInfo), typeof(object)),
                                param
                            );
                            query = query.OrderBy(expr);
                        }
                    }
                    else
                    {
                        var propertyInfo = typeof(T).GetProperty(sortColumn);
                        if (propertyInfo != null)
                        {
                            var param = Expression.Parameter(typeof(T));
                            var expr = Expression.Lambda<Func<T, object>>(
                                Expression.Convert(Expression.Property(param, propertyInfo), typeof(object)),
                                param
                            );
                            query = query.OrderByDescending(expr);
                        }
                    }
                }
                */
            }
            if (page <= 0)
                page = 1;
            if (pageSize <= 0)
                pageSize = 20;

            if (orderBy != null)
                query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return query.AsEnumerable();
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public T GetById(Y id)
        {
            return dbSet.Find(id);
        }

        //Remove By ID
        public void Remove(Y id)
        {
            T entity = dbSet.Find(id);
            dbSet.Remove(entity);
            // _context.SaveChanges();
        }

        //Remove

        public void Remove(T entity)
        {
            if (asNoTracking)
                dbSet.Remove(entity);
            else
                _context.Entry(entity).State = EntityState.Deleted;
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            //_context.SaveChanges();
        }

        public void Update()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransAsync(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified
        )
        {
            Transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
            return Transaction;
        }

        public IDbContextTransaction BeginTrans(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified
        )
        {
            Transaction = _context.Database.BeginTransaction(isolationLevel);
            return Transaction;
        }

        public async Task CommitAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
           await Transaction?.CommitAsync();
        }

        public async Task RollbackAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            await Transaction?.RollbackAsync();
        }
    }
}
