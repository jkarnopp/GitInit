using GitInitTest.Common.Repository;
using GitInitTest.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitInitTest.Data.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        //UserInformation GetUserWithRoles(string UserLanId);
    }

    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        //public DbContext context
        //{
        //    get { return db as DbContext; }
        //}

        public RoleRepository(DbContext db) : base(db)
        {
        }
    }
}