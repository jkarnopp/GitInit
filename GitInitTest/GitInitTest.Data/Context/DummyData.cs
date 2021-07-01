using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace GitInitTest.Data.Context
{
    public class DummyData
    {
        public static async Task Initialize(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            String adminId1 = "";
            String adminId2 = "";

            string role1 = "Admin";
            string desc1 = "This is the administrator role";

            string role2 = "CompanyAdmin";
            string desc2 = "This is the administrator role for each company";

            string role3 = "Employee";
            string desc3 = "This is the employee role for each company";

            string password = "B0z0built!";

            if (await roleManager.FindByNameAsync(role1) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role1, desc1, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(role2) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role2, desc2, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(role3) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(role3, desc3, DateTime.Now));
            }

            if (await userManager.FindByNameAsync("jkarnopp") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "jkarnopp",
                    Email = "jim@kartech.com",
                    FirstName = "Jim",
                    LastName = "Karnopp",
                    PhoneNumber = "7344178992",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role1);
                }
                adminId1 = user.Id.ToString();
            }

            if (await userManager.FindByNameAsync("fkarnopp") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "fkarnopp",
                    Email = "forrest@forrestforhire.com",
                    FirstName = "Forrest",
                    LastName = "Karnopp",
                    PhoneNumber = "7788951456",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, role2);
                }
                adminId2 = user.Id.ToString();
            }
        }
    }
}