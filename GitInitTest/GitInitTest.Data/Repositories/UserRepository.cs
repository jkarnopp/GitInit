using GitInitTest.Common.Repository;
using GitInitTest.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GitInitTest.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserWithRoles(string UserLanId);

        IEnumerable<User> GetUserListWithRoles(Expression<Func<User, bool>> filter, Func<IQueryable<User>, IOrderedQueryable<User>> orderBy);

        Task<List<User>> GetUserListWithRolesAsync(Expression<Func<User, bool>> filter,
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy);
    }

    public class UserRepository : Repository<User>, IUserRepository
    {
        //public DbContext context
        //{
        //    get
        //    {
        //        return db as DbContext;
        //    }
        //}
        public UserRepository(DbContext db) : base(db)
        {
        }

        public User GetUserWithRoles(string email)
        {
            return db.Set<User>().AsNoTracking().Where(u => u.Email == email)
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role).SingleOrDefault();
        }

        public virtual IEnumerable<User> GetUserListWithRoles(
                   Expression<Func<User, bool>> filter = null,
                   Func<IQueryable<User>, IOrderedQueryable<User>> orderBy = null
                   )
        {
            IQueryable<User> query = db.Set<User>()
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role);

            if (filter != null)
            {
                query = query.Where(filter);
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

        public Task<List<User>> GetUserListWithRolesAsync(Expression<Func<User, bool>> filter,
            Func<IQueryable<User>, IOrderedQueryable<User>> orderBy)
        {
            IQueryable<User> query = db.Set<User>()
                .Include(ur => ur.UserRoles)
                .ThenInclude(r => r.Role);

            if (filter != null)
            {
                query = query.Where(filter);
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
    }
}