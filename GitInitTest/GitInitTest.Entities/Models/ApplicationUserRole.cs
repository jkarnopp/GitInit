using Microsoft.AspNetCore.Identity;
using System;

namespace GitInitTest.Entities.Models
{
    //Added from article on Stack
    //https://stackoverflow.com/questions/51004516/net-core-2-1-identity-get-all-users-with-their-associated-roles/51005445
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}