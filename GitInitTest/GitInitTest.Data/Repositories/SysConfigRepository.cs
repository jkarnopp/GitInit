using GitInitTest.Common.Repository;
using GitInitTest.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GitInitTest.Data.Repositories
{
    public interface ISysConfigRepository : IRepository<SysConfig>
    {
        ValueTask<SysConfig> GetSysConfigAsync();
    }

    public class SysConfigRepository : Repository<SysConfig>, ISysConfigRepository
    {
        public DbContext context
        {
            get
            {
                return db as DbContext;
            }
        }

        public SysConfigRepository(DbContext _db) : base(_db)
        {
        }

        public ValueTask<SysConfig> GetSysConfigAsync()
        {
            return db.Set<SysConfig>().FindAsync(1);
        }
    }
}