using GitInitTest.Common.Email;
using GitInitTest.Common.Email;
using GitInitTest.Common.Helpers;
using GitInitTest.Data.Repositories;
using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace GitInitTest.Site.Services
{
    public interface IUserInformationAdminService
    {
        Task<UserInfoAdminIndexViewModel> GetIndexViewAsync(UserInformationAdminSearchDto userInformationSearchDto,
            PageListActionDto pageListActionDto, Guid? id);

        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        Task<ApplicationUserDto> GetByIdAsync(Guid? id);

        UserInfoAdminEditViewModel GetCreateView();

        Task<UserInfoAdminEditViewModel> GetEditViewAsync(string userName, string title);

        UserInfoAdminEditViewModel GetEditView(ApplicationUser userInformation, string title);

        Task<bool> CheckUserAsync(string lanId);

        Task CreateUserAsync(RegisterUserDto userInformation, string[] selectedUserRoles);

        Task UpdateUserAsync(ApplicationUser userInformation, string[] selectedUserRoles);

        Task DeleteUser(string id);

        Task NotifyForNewUser(RegisterUserDto userInformation);

        AdminPwResetViewModel GetAdminPwResetView(string userName);

        Task AdminPwReset(AdminPwResetDto adminPwResetDto);
    }

    public class UserInformationAdminService : IUserInformationAdminService
    {
        private IUnitOfWork _unitOfWork;
        private IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IUserResolverService _userResolverService;
        private string _TitleDesc = " User";

        public UserInformationAdminService(IUnitOfWork unitOfWork, IEmailService emailService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUserResolverService userResolverService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            //_userManager = userManager;
            _roleManager = roleManager;
            _userResolverService = userResolverService;
        }

        public async Task<UserInfoAdminIndexViewModel> GetIndexViewAsync(UserInformationAdminSearchDto searchDto,
            PageListActionDto pageListActionDto, Guid? id)
        {
            var indexViewModel = new UserInfoAdminIndexViewModel();

            if (pageListActionDto == null) pageListActionDto = new PageListActionDto();
            if (searchDto == null) searchDto = new UserInformationAdminSearchDto();

            indexViewModel.SearchDto = searchDto;

            //This section used to set ascending or descending order
            indexViewModel.SortDto.FirstName =
                String.IsNullOrEmpty(pageListActionDto.SortOrder) ? "FirstName_desc" : "";

            indexViewModel.SortDto.LastName =
                pageListActionDto.SortOrder == "LastName" ? "LastName_desc" : "LastName";

            indexViewModel.SortDto.UserName =
                pageListActionDto.SortOrder == "UserName" ? "UserName_desc" : "UserName";

            indexViewModel.Title = "Manage " + _TitleDesc + "s";
            pageListActionDto.PageSize = 10;

            indexViewModel.PageListAction.CurrentSort = pageListActionDto.SortOrder;

            //var userInformations = await _unitOfWork.ApplicationUserRepository.GetAsync(ExpressionBuilder<ApplicationUser>.GetExpression(searchDto),
            //    GetSortOrder(pageListActionDto.SortOrder), "");

            var userInformations = await _unitOfWork.ApplicationUserRepository.GetAsync(await UserSearch(searchDto),
                GetSortOrder(pageListActionDto.SortOrder), "");

            indexViewModel.UserInformations = await userInformations
                .ToPagedListAsync((pageListActionDto.Page ?? 1), pageListActionDto.PageSize);

            //Add items for selected user
            if (id != null)
            {
                var selectedUserInformation = await _unitOfWork.ApplicationUserRepository.SingleOrDefaultAsync(
                    u => u.Id == id, "ApplicationUserUserRoles,ApplicationUserUserRoles.ApplicationUserRole");

                indexViewModel.SelectedUserInformation = selectedUserInformation;
            }

            return indexViewModel;
        }

        private async Task<Expression<Func<ApplicationUser, bool>>> UserSearch(UserInformationAdminSearchDto searchDto)
        {
            var predicate = PredicateBuilder.New<ApplicationUser>(true);
            //if (searchDto.IsCreditIssued == true)
            //    predicate = predicate.Or(p => p.ClaimStatusId == (int)ClaimStatus.CreditIssued);
            //if (searchDto.IsApproved == true)
            //    predicate = predicate.Or(p => p.ClaimStatusId == (int)ClaimStatus.Approved);
            //if (searchDto.IsDenied == true)
            //    predicate = predicate.Or(p => p.ClaimStatusId == (int)ClaimStatus.Denied);
            //if (searchDto.IsCurrent == true || (!searchDto.IsApproved == true && !searchDto.IsDenied == true))
            //    predicate = predicate.Or(p => p.ClaimStatusId > (int)ClaimStatus.Approved);

            //if (!string.IsNullOrEmpty(searchDto.MtuSalesOrderNumber))
            //    predicate = predicate.And(s => EF.Functions.Like(s.MtuSalesOrderNumber.ToString(), searchDto.MtuSalesOrderNumber.Replace("*", "%")));

            //if (!string.IsNullOrEmpty(searchDto.DistributorInfoId))
            //    predicate = predicate.And(s => EF.Functions.Like(s.DistributorInfoId.ToString(), searchDto.DistributorInfoId.Replace("*", "%")));

            //if (!await _userResolverService.IsUserInRoleAsync("Administrator,Approver"))
            //{
            //    var user = await _userResolverService.GetCurrentUserAsync();
            //    predicate = predicate.And(p =>
            //        user.UserDistributors.Select(d => d.DistributorInfo).Select(c => c.DistributorInfoId).ToList()
            //            .Contains(p.DistributorInfoId));
            //}


            if (!string.IsNullOrEmpty(searchDto.LastName))
                predicate = predicate.And(s => EF.Functions.Like(s.LastName, searchDto.LastName.Replace("*", "%")));

            if (!await _userResolverService.IsUserInRoleAsync("Administrator,Approver"))
            {
            }

            return predicate;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _unitOfWork.ApplicationUserRepository.GetAllAsync();
        }

        public async Task<ApplicationUserDto> GetByIdAsync(Guid? id)
        {
            var userInformation = await _unitOfWork.ApplicationUserRepository.SingleOrDefaultAsync(u => u.Id == id, "UserRoles,UserRoles.Role");

            return new ApplicationUserDto(userInformation);
        }

        public UserInfoAdminEditViewModel GetCreateView()
        {
            var userInformation = new ApplicationUser();

            string[] selectedUserRoles = new string[0];

            return GetEditView(userInformation, "Create User");
        }

        //public UserInfoAdminEditViewModel GetEditView(string email, string title)
        //{
        //    var userInformationId = _unitOfWork.UserRepository.GetAsync(u => u.Email == email).Result.Select(i => i.UserId).SingleOrDefault();
        //    if (userInformationId == 0) throw new ApplicationException("User ID not found in Database");
        //    return GetEditView(userInformationId, title);
        //}

        public async Task<UserInfoAdminEditViewModel> GetEditViewAsync(string userName, string title)
        {
            //var userInformation = _unitOfWork.UserRepository.FindAsync(u => u.UserId == id).Result.SingleOrDefault();
            //var userInformation = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(u => u.UserName == id);
            var userInformation = _userResolverService.GetDetailByUserName(userName);

            if (userInformation == null) throw new ApplicationException("User ID not found in Database");

            //string[] selectedUserRoles = _unitOfWork.UserRoleRepository.FindAsync(u => u.UserId == id).Result
            //    .Select(r => r.RoleId.ToString()).ToArray();

            return GetEditView(userInformation, title);
        }

        public UserInfoAdminEditViewModel GetEditView(ApplicationUser userInformation, string title)
        {
            var userInfoAdminEditViewModel = new UserInfoAdminEditViewModel();
            userInfoAdminEditViewModel.Title = title;
            userInfoAdminEditViewModel.UserInformation = new RegisterUserDto(userInformation);
            var assignedUserRoles = userInformation?.UserRoles?.Select(r => r.Role)?.ToArray();
            userInfoAdminEditViewModel.AssignedRoleDatas = _userResolverService.GetAssignedUserRoles(assignedUserRoles);

            return userInfoAdminEditViewModel;
        }

        public async Task<bool> CheckUserAsync(string email)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email);
            if (!user.Any()) return false;
            return true;
        }

        public async Task CreateUserAsync(RegisterUserDto userInformation, string[] selectedUserRoles)
        {
            //_unitOfWork.UserRepository.Add(userInformation);
            //_unitOfWork.SaveChanges();
            //var userAddResult = _userManager.CreateAsync(userInformation).Result;

            //if (userAddResult.Succeeded)
            //{
            //    AddRolesForUser(userInformation.UserName, selectedUserRoles);
            //}
            await _userResolverService.CreateUserAsync(userInformation, selectedUserRoles);
            await NotifyForNewUser(userInformation);
        }

        public async Task UpdateUserAsync(ApplicationUser userInformation, string[] selectedUserRoles)
        {
            await _userResolverService.UpdateUserAsync(userInformation, selectedUserRoles);
            //var user = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(u => u.UserName == userInformation.UserName);

            //user.LastName = userInformation.LastName;
            //user.FirstName = userInformation.FirstName;
            //user.Email = userInformation.Email;

            //var userUpdate = _userManager.UpdateAsync(user).Result;

            //if (userUpdate.Succeeded) AddRolesForUser(userInformation.UserName, selectedUserRoles);
        }

        public async Task DeleteUser(string id)
        {
            await _userResolverService.DeleteUser(id);
        }

        /// <summary>
        /// This method uses MailLink service to sent email to access approver
        /// with a link to the edit page where the approval can be set for the user.
        /// </summary>
        /// <param name="userInformation"></param>
        /// <param name="hostingEnvironmentWebRootPath"></param>
        public async Task NotifyForNewUser(RegisterUserDto userInformation)
        {
            var sysConfig = await _unitOfWork.SysConfigRepository.GetSysConfigAsync();
            var fromAddress = new EmailAddress
            {
                Address = sysConfig.AppFromEmail,
                Name = sysConfig.AppFromName
            };
            var toAddress = new EmailAddress
            {
                Address = userInformation.Email,
                Name = userInformation.FirstName + " " + userInformation.LastName
            };
            //var linkUrl = hostingEnvironmentWebRootPath + @"/UserInformationAdmin/edit/" +
            //              userInformation.UserId;
            var emailMessage = new EmailMessage();
            emailMessage.FromAddresses.Add(fromAddress);
            emailMessage.ToAddresses.Add(toAddress);
            emailMessage.Subject = "New User Request for Work Master Pro";
            emailMessage.Content =
                $"A new user has been made for {userInformation.FirstName} {userInformation.LastName} " +
                $"in the RestaurantSpecialsTodaySystem. Please change your password.";

            _emailService.Send(emailMessage);
        }

        public AdminPwResetViewModel GetAdminPwResetView(string userName)
        {
            return new AdminPwResetViewModel(userName);
        }

        public async Task AdminPwReset(AdminPwResetDto adminPwResetDto)
        {
            await _userResolverService.AdminResetPassword(adminPwResetDto.UserName, adminPwResetDto.Password);
        }

        //******* This Comment Saved in case Dot Net Core supports Many to Many ******
        //private List<AssignedRoleData> PopulateAssignedRoleData(UserInformation userInformation)
        //{
        //    var allUserRoles = _unitOfWork.UserRoles.GetAll();
        //    var userInformationUserRoles = new HashSet<int>(userInformation.UserInformationUserRoles.Select(r => r.UserRoleId));
        //    var assignedRoleDataDto = new List<AssignedRoleData>();
        //    foreach (var role in allUserRoles)
        //    {
        //        assignedRoleDataDto.Add(new AssignedRoleData
        //        {
        //            UserRoleId = role.UserRoleId,
        //            Name = role.Name,
        //            Assigned = userInformationUserRoles.Contains(role.UserRoleId)
        //        });
        //    }
        //   return assignedRoleDataDto;
        //}

        private Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> GetSortOrder(string sortOrder)
        {
            switch (sortOrder)
            {
                case "FirstName_desc":
                    return q => q.OrderByDescending(v => v.FirstName);

                case "LastName":
                    return q => q.OrderBy(v => v.LastName);

                case "LastName_desc":
                    return q => q.OrderByDescending(v => v.LastName);
                case "UserName":
                    return q => q.OrderBy(v => v.UserName);

                case "UserName_desc":
                    return q => q.OrderByDescending(v => v.UserName);

                default:
                    return q => q.OrderBy(v => v.FirstName);
            }
        }
    }
}