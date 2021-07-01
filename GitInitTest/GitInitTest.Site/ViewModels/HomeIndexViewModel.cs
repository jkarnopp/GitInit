using GitInitTest.Entities.Models;

namespace GitInitTest.Site.ViewModels
{
    public class HomeIndexViewModel : BaseViewModel
    {
        public ApplicationUser User { get; set; }
        public bool IsEmployeeProfileConfigured { get; set; }
        public bool IsCompanyConfigured { get; set; }
    }
}