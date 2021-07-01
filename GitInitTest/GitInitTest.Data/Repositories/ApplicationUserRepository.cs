using GitInitTest.Common.Repository;
using GitInitTest.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace GitInitTest.Data.Repositories
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
    }

    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(DbContext db) : base(db)
        {
        }
    }
}