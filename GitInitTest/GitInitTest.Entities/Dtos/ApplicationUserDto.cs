using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitInitTest.Entities.Dtos
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser f)
        {
            Id = f.Id;
            Username = f.UserName;
            FirstName = f.FirstName;
            LastName = f.LastName;
            EmailAddress = f.Email;
            //Enabled = f.Enabled;
            UserRoles = f.UserRoles?.Select(s => s.Role).ToList();
        }

        public List<ApplicationRole> UserRoles { get; set; }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        //public bool Enabled { get; set; }
    }
}