using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GitInitTest.Site.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class UserInformationAdminController : Controller
    {
        private readonly IUserInformationAdminService _service;

        public UserInformationAdminController(IUserInformationAdminService userInformationAdminService)
        {
            _service = userInformationAdminService;
        }

        // GET: UserInformationAdmin
        //public async Task<IActionResult> Index([Bind("LastName")]UserSearchDto userSearchDto)
        //{
        //    if (userSearchDto == null) userSearchDto = new UserSearchDto();

        //    //Todo: make the service methods asynchronous
        //    var userView = _userInformationAdminService.GetIndexView(userSearchDto);
        //    return View(userView);
        //}

        [HttpPost, HttpGet]
        public async Task<IActionResult> Index(
            [Bind("LastName")] UserInformationAdminSearchDto searchDto = null,
            [Bind("SortOrder,CurrentSort,CurrentFilter,SearchString,IncludedProperties,Page,PageNumber,PageSize")] PageListActionDto pageListActionDto = null,
            Guid? id = null)
        {
            var viewModel = await
                _service.GetIndexViewAsync(searchDto, pageListActionDto, id);

            return View(viewModel);
        }

        //public async Task<IActionResult> Index(
        //    [Bind("CiShortDesc,CiActive,CiDeleted,CId,parm_ShortDescSort,parm_DescSort")] CompanySearchDto ciCatalogItemSearchDto = null,
        //    [Bind("SortOrder,CurrentSort,CurrentFilter,SearchString,IncludedProperties,Page,PageNumber,PageSize")] PageListActionDto pageListActionDto = null)
        //{
        //    var viewModel = await
        //        _service.GetIndexViewAsync(ciCatalogItemSearchDto, pageListActionDto);

        //    return View(viewModel);
        //}

        // GET: UserInformationAdmin/Create
        public IActionResult Create()
        {
            var createView = _service.GetCreateView();
            return View(createView);
        }

        // POST: UserInformationAdmin/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CompanyId,FirstName,LastName,Email,Enabled")] User userInformation, string[] selectedUserRoles)
        //{
        //    if (userInformation.Email != null && _userInformationAdminService.CheckUser(userInformation.Email))
        //    {
        //        ModelState.AddModelError("LanId", "LanId Already Exists");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _userInformationAdminService.CreateUser(userInformation, selectedUserRoles);

        //        return RedirectToAction(nameof(Index));
        //    }
        //    var createView = _userInformationAdminService.GetEditView(userInformation, selectedUserRoles, "Create User");
        //    return View(createView);
        //}

        // GET: UserInformationAdmin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editView = await _service.GetEditViewAsync(id, "Edit User");

            return View(editView);
        }

        //POST: UserInformationAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName,FirstName,LastName,Email,PhoneNumber,Password,ConfirmPassword,IsEnabled,EmailConfirmed")] RegisterUserDto userInformation, string[] selectedUserRoles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(userInformation);
                    //await _context.SaveChangesAsync();
                    await _service.CreateUserAsync(userInformation, selectedUserRoles);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var editView = _service.GetEditView(new ApplicationUser(userInformation), "Create User");
            return View(editView);
        }

        //POST: UserInformationAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,FirstName,LastName,Email,PhoneNumber,IsEnabled,EmailConfirmed")] ApplicationUser userInformation, string[] selectedUserRoles)
        {
            if (id != userInformation.UserName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(userInformation);
                    //await _context.SaveChangesAsync();
                    await _service.UpdateUserAsync(userInformation, selectedUserRoles);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var editView = _service.GetEditView(userInformation, "Edit User");
            return View(editView);
        }

        // GET: UserInformationAdmin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editView = await _service.GetEditViewAsync(id, "Delete User");

            return View(editView);
        }

        // POST: UserInformationAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AdminPwReset(string id)
        {
            var adminPwResetView = _service.GetAdminPwResetView(id);
            return View(adminPwResetView);
        }

        // POST: UserInformationAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminPwReset([Bind("UserName,Password")] AdminPwResetDto adminPwResetDto)
        {
            if (ModelState.IsValid)
            {
                await _service.AdminPwReset(adminPwResetDto);

                return RedirectToAction(nameof(Index));
            }
            var adminPwResetView = _service.GetAdminPwResetView(adminPwResetDto.UserName);
            return View(adminPwResetView);
        }
    }
}