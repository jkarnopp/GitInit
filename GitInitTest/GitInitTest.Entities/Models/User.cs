using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitInitTest.Entities.Models
{
    public class User
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [MaxLength(50), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50), Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }

        public bool Enabled { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }


    }
}