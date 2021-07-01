using GitInitTest.Entities.Models;

namespace GitInitTest.Entities.Dtos
{
    public class UserDto
    {
        public UserDto(User f)
        {
            UserId = f.UserId;
            Username = f.Username;
            FirstName = f.FirstName;
            LastName = f.LastName;
            Email = f.Email;
            Enabled = f.Enabled;

        }

        public long UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool Enabled { get; set; }


    }
}
