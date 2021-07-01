using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitInitTest.Site.ViewModels
{
    public class UserInfoAdminEditViewModel : BaseViewModel
    {
        public RegisterUserDto UserInformation { get; set; }

        //public IEnumerable<SelectListItem> Company { get; set; }
        public List<AssignedRoleData> AssignedRoleDatas { get; set; }
    }
}