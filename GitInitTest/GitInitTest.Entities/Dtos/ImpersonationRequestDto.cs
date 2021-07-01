using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitInitTest.Entities.Dtos
{
    public class ImpersonationRequestDto
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
