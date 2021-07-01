using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitInitTest.Entities.Models
{
    public class Role
    {
        [Key]
        public long RoleId { get; set; }

        [MaxLength(25)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}