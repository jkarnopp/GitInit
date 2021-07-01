using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace GitInitTest.Site.ViewModels
{
    public class AdminPwResetViewModel : BaseViewModel
    {
        public AdminPwResetViewModel()
        {
        }

        public AdminPwResetViewModel(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}