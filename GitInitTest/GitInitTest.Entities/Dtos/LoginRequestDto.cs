using GitInitTest.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace GitInitTest.Entities.Dtos
{
    public class LoginRequestDto
    {
        public LoginRequestDto(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        public LoginRequestDto()
        {

        }

        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
