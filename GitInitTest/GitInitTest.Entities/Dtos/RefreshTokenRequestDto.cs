using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitInitTest.Entities.Dtos
{
    public class RefreshTokenRequestDto
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
