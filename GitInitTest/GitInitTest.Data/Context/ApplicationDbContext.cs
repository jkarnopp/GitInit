using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace GitInitTest.Data.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../GitInitTest.Site/appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new ApplicationDbContext(builder.Options);
        }
    }

    //public partial class ApplicationContext : DbContext
    //{
    //    public ApplicationContext()
    //    {
    //    }

    //    public ApplicationContext(DbContextOptions<ApplicationContext> options)
    //        : base(options)
    //    {
    //    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>,
    ApplicationUserRole, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SysConfig> SysConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<SysConfig>().HasData(
                new SysConfig
                {
                    AppFromEmail = "GitInitTest.Data@noReply.kartech.com",
                    AppFromName = "GitInitTest.Data",
                    AppName = "GitInitTest.Data",
                    BusinessOwnerEmail = "jim@kartech.com",
                    BusinessOwnerName = "Jim Karnopp",
                    DeveloperEmail = "jim@kartech.com",
                    DeveloperName = "Jim Karnopp",
                    SmtpPort = 25,
                    SmtpServer = "smtp.kartech.com",
                    SysConfigId = 1,
                    IsDebug = true
                }
            );
        }
    }
}