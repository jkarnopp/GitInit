using GitInitTest.Entities.Dtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace GitInitTest.Entities.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(RegisterUserDto registerUserDto) : base()
        {
            FirstName = registerUserDto.FirstName;
            LastName = registerUserDto.LastName;
            Email = registerUserDto.Email;
            PhoneNumber = registerUserDto.PhoneNumber;
            UserName = registerUserDto.UserName;
            IsEnabled = registerUserDto.IsEnabled;
            EmailConfirmed = registerUserDto.EmailConfirmed;


        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEnabled { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}