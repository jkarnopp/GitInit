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
    public interface IApplicationRoleRepository : IRepository<ApplicationRole>
    {
    }

    public class ApplicationRoleRepository : Repository<ApplicationRole>, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(DbContext db) : base(db)
        {
        }
    }
}