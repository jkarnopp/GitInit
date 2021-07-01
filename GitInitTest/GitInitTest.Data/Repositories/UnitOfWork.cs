using GitInitTest.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GitInitTest.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IRoleRepository RoleRepository { get; }

        IApplicationUserRepository ApplicationUserRepository { get; }
        IApplicationUserRoleRepository ApplicationUserRoleRepository { get; }
        IApplicationRoleRepository ApplicationRoleRepository { get; }

        ISysConfigRepository SysConfigRepository { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// The Unit of Work pattern allows you to share a single DbContext among several queries allow you to rollback if one fails.
    /// For each repository, create a new entry in the interface and copy one of the get sections below changing the type information.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _db;

        public UnitOfWork(DbContext db)
        {
            _db = db;
        }

        private ISysConfigRepository _sysConfigRepository;

        public ISysConfigRepository SysConfigRepository => _sysConfigRepository ??= new SysConfigRepository(_db);

        private IApplicationUserRoleRepository _applicationUserRoleRepository;

        public IApplicationUserRoleRepository ApplicationUserRoleRepository => _applicationUserRoleRepository ??= new ApplicationUserRoleRepository(_db);

        private IApplicationRoleRepository _applicationRoleRepository;

        public IApplicationRoleRepository ApplicationRoleRepository => _applicationRoleRepository ??= new ApplicationRoleRepository(_db);

        private IApplicationUserRepository _applicationUserRepository;

        public IApplicationUserRepository ApplicationUserRepository => _applicationUserRepository ??= new ApplicationUserRepository(_db);

        private IUserRoleRepository _userRoleRepository;

        public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_db);

        private IRoleRepository _roleRepository;

        public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_db);

        private IUserRepository _userRepository;

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_db);

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}