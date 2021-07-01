using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using System.Collections.Generic;
using X.PagedList;

namespace GitInitTest.Site.ViewModels
{
    public class UserInfoAdminIndexViewModel : BaseViewModel
    {
        public UserInfoAdminIndexViewModel()
        {
            SearchDto = new UserInformationAdminSearchDto();
            SortDto = new UserInformationAdminSortDto();
            PageListAction = new PageListActionDto();
        }

        public ApplicationUser SelectedUserInformation { get; set; }

        public IPagedList<ApplicationUser> UserInformations { get; set; }

        public PageListActionDto PageListAction { get; set; }
        public UserInformationAdminSearchDto SearchDto { get; set; }
        public UserInformationAdminSortDto SortDto { get; set; }

        public IEnumerable<ApplicationRole> UserRoles { get; set; }
    }
}