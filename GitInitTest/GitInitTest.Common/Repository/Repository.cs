using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GitInitTest.Common.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
                                Expression<Func<TEntity, bool>> filter = null,
                                Func<IQueryable<TEntity>,
                                IOrderedQueryable<TEntity>> orderBy = null,
                                string includeProperties = ""
                                );

        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        IEnumerable<TEntity> GetAll(string includeProperties = "");

        Task<List<TEntity>> GetAllAsync(string includeProperties = "");

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        TEntity Find(object Id);

        ValueTask<TEntity> FindAsync(object Id);

        void Add(TEntity entity);

        ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        Task AddRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void ExplicitUpdate(TEntity entity);

        void Remove(object Id);

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);
    }

    /// <summary>
    /// This class takes in a type and generates the CRUD actions for that type.
    /// This class can be used by itself, but more likely will be inherited to add additional queries.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext db;
        private readonly DbSet<TEntity> dbSet;

        public Repository(DbContext _db)
        {
            db = _db;
            dbSet = db.Set<TEntity>();
        }

        /// <summary>
        /// This function takes in three parameters.
        /// </summary>
        /// <param name="filter">a => a.Name.StartsWith("StartText")</param>
        /// <param name="orderBy">orderBy: q => q.OrderBy(d => d.Name)</param>
        /// <param name="includeProperties">Comma Separated related objects</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
                   Expression<Func<TEntity, bool>> filter = null,
                   Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                   string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsync();
            }
            else
            {
                return query.ToListAsync();
            }
        }

        private IQueryable<TEntity> GetIncludes(IQueryable<TEntity> query, string includeProperties = "")
        {
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public IEnumerable<TEntity> GetAll(string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().ToList();
            return query.ToList();
        }

        public Task<List<TEntity>> GetAllAsync(string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().ToListAsync();
            return query.ToListAsync();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().SingleOrDefault(predicate);
            return query.SingleOrDefault(predicate);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().SingleOrDefaultAsync(predicate);
            return query.SingleOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().FirstOrDefault(predicate);
            return query.FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            IQueryable<TEntity> query = db.Set<TEntity>();
            query = GetIncludes(query, includeProperties);
            //return db.Set<TEntity>().FirstOrDefaultAsync(predicate);
            return query.FirstOrDefaultAsync(predicate);
        }

        public TEntity Find(object Id)
        {
            return db.Set<TEntity>().Find(Id);
        }

        public ValueTask<TEntity> FindAsync(object Id)
        {
            return db.Set<TEntity>().FindAsync(Id);
        }

        public void Add(TEntity entity)
        {
            db.Set<TEntity>().Add(entity);
        }

        public ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            return db.Set<TEntity>().AddAsync(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            db.Set<TEntity>().AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            return db.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            db.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            db.Set<TEntity>().RemoveRange(entities);
        }

        public void Remove(object Id)
        {
            TEntity entity = db.Set<TEntity>().Find(Id);
            this.Remove(entity);
        }

        public void ExplicitUpdate(TEntity entity)
        {
            dbSet.Attach(entity);
            var entry = db.Entry(entity);
            //Update(entry);
            entry.State = EntityState.Modified;
        }

        public void Update(TEntity entity)
        {
            db.Entry(entity).State = EntityState.Modified;
        }
    }
}