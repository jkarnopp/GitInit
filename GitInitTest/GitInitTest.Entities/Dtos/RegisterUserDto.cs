using GitInitTest.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GitInitTest.Entities.Dtos
{
    public class RegisterUserDto
    {
        public RegisterUserDto()
        {

        }
        public RegisterUserDto(ApplicationUser applicationUser)
        {
            UserName = applicationUser.UserName;
            FirstName = applicationUser.FirstName;
            LastName = applicationUser.LastName;
            PhoneNumber = applicationUser.PhoneNumber;
            Email = applicationUser.Email;
            IsEnabled = applicationUser.EmailConfirmed;
            EmailConfirmed = applicationUser.EmailConfirmed;
        }
        [Required]
        [Display(Name = "User ID")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Is User Enabled")]
        public bool IsEnabled { get; set; }
        [Display(Name = "Is Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        public Guid Id { get; set; }
    }
}