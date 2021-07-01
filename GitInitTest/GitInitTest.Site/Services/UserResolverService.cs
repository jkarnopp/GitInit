using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitInitTest.Site.Services
{
    public interface IUserResolverService
    {
        Task<ApplicationUser> GetCurrentUserAsync();

        Task<ApplicationUser> GetByUserNamAsync(string userName);

        ApplicationUser GetDetailByUserName(string userName);

        Task CreateUserAsync(RegisterUserDto userInformation, string[] selectedUserRoles);

        Task UpdateUserAsync(ApplicationUser userInformation, string[] selectedUserRoles);

        Task DeleteUserAsync();

        Task DeleteUser(string userName);

        List<AssignedRoleData> GetAssignedUserRoles(ICollection<ApplicationRole> selectedUserRoles);

        Task<bool> IsUserInRoleAsync(string roleName);

        Task AddRolesForUser(string userId, string[] selectedUserRoles);

        Task AdminResetPassword(string userName, string password);
    }

    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserResolverService(IHttpContextAccessor context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByNameAsync(_context.HttpContext.User?.Identity?.Name);
        }

        public async Task<ApplicationUser> GetByUserNamAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public ApplicationUser GetDetailByUserName(string userName)
        {
            return _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(u => u.UserName == userName);
        }

        public async Task CreateUserAsync(RegisterUserDto Input, string[] selectedUserRoles)
        {
            var user = new ApplicationUser
            {
                UserName = Input.UserName,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                PhoneNumber = Input.PhoneNumber,
                EmailConfirmed = Input.EmailConfirmed,
                IsEnabled = Input.IsEnabled

            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await AddRolesForUser(user.UserName, selectedUserRoles);

                //_logger.LogInformation("User created a new account with password.");
            }
        }

        public async Task UpdateUserAsync(ApplicationUser userInformation, string[] selectedUserRoles)
        {
            //var user = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(u => u.UserName == userInformation.UserName);
            var user = await _userManager.FindByNameAsync(userInformation.UserName);
            //user.LastName = userInformation.LastName;
            //user.FirstName = userInformation.FirstName;
            //user.Email = userInformation.Email;

            //var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return;
            }

            if (user.FirstName != userInformation.FirstName) user.FirstName = userInformation.FirstName;
            if (user.LastName != userInformation.LastName) user.LastName = userInformation.LastName;

            var x = await _userManager.UpdateAsync(user);
            if (!x.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                throw new InvalidOperationException($"Unexpected error occurred setting First Name and Last Name for user with ID '{userId}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (userInformation.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, userInformation.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (userInformation.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userInformation.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (await _userManager.IsEmailConfirmedAsync(user) != true && userInformation.EmailConfirmed == true)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await _userManager.ConfirmEmailAsync(user, token);
            }

            //var userUpdate = await _userManager.UpdateAsync(user);

            await AddRolesForUser(userInformation.UserName, selectedUserRoles);
        }

        public Task DeleteUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteUser(string userName)
        {
            if (userName == null)
            {
                throw new ApplicationException("User not provided");
            }

            //get User Data from Userid
            var user = await _userManager.FindByNameAsync(userName);

            //List Logins associated with user
            var logins = await _userManager.GetLoginsAsync(user);

            //Gets list of Roles associated with current user
            var rolesForUser = await _userManager.GetRolesAsync(user);

            foreach (var login in logins.ToList())
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var result = await _userManager.RemoveFromRoleAsync(user, item);
                }
            }

            //Delete User
            var deleteResult = _userManager.DeleteAsync(user).Result;
            if (deleteResult.Succeeded)
            {
                Log.Information("User: " + userName + " Deleted");
            }

            return;
        }

        /// <summary>
        /// This method builds the list that can be used to have populated
        /// checkboxes on the view.
        /// </summary>
        /// <param name="selectedUserRoles"></param>
        /// <returns></returns>
        public List<AssignedRoleData> GetAssignedUserRoles(ICollection<ApplicationRole> selectedUserRoles)
        {
            var allUserRoles = _roleManager.Roles;
            var assignedRoleDataDto = new List<AssignedRoleData>();


            foreach (var role in allUserRoles)
            {
                var assigned = false;
                if (selectedUserRoles != null) assigned = selectedUserRoles.Select(i => i.Id).Contains(role.Id);

                assignedRoleDataDto.Add(new AssignedRoleData
                {
                    UserRoleId = role.Id,
                    Name = role.Name,
                    Assigned = assigned
                });
            }


            return assignedRoleDataDto;
        }

        public async Task<bool> IsUserInRoleAsync(string roleName)
        {
            var user = await GetCurrentUserAsync();

            var roles = await _userManager.GetRolesAsync(user);

            var roleNames = roleName.Split(",");

            foreach (var name in roleNames)
            {
                if (roles.Contains(name.Trim())) return true;
            }

            return false;
        }

        /// <summary>
        /// This method first deletes all the existing roles for a user then
        /// adds the selected roles back in the database. This has to be done
        /// with the intermediary table because .Net core doesn't support
        /// Many to Many relationships yet.
        /// </summary>
        /// <param name="userInformationId"></param>
        /// <param name="selectedUserRoles"></param>
        public async Task AddRolesForUser(string userId, string[] selectedUserRoles)
        {
            //var user = _userManager.Users.Include(ur => ur.UserRoles)
            //    .ThenInclude(r => r.Role)
            //    .FirstOrDefault(u => u.UserName == userId);

            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
            {
                return;
            }

            var rolesToDelete = await _userManager.GetRolesAsync(user);
            //var rolesToDelete = user.UserRoles.Select(r => r.Role.Name);

            //var rolesToDelete = user.UserRoles.Select(r => r.Role).Select(rn => rn.Name).ToList();
            //var rolesToDelete = await _roleManager.

            var removeFromRolesAsync = await _userManager.RemoveFromRolesAsync(user, rolesToDelete);

            if (removeFromRolesAsync.Succeeded)
            {
                var addToRolesAsync = await _userManager.AddToRolesAsync(user, selectedUserRoles);
                if (addToRolesAsync.Succeeded) return;
            }

            //_unitOfWork.UserRoleRepository.RemoveRange(rolesToDelete);

            //foreach (var selectedUserRole in selectedUserRoles)
            //{
            //    _unitOfWork.UserRoleRepository.Add(new UserRole { UserId = userInformationId, RoleId = int.Parse(selectedUserRole) });
            //}

            //_unitOfWork.SaveChanges();
        }

        public async Task AdminResetPassword(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, password);
        }
    }
}