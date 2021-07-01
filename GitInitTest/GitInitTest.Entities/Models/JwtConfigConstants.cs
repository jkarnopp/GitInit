using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GitInitTest.Entities.Models
{
    public class JwtConfigConstants
    {

        public const string Issuer = "Kartech";
        public const string Audience = "ApiUser";
        public const string Key = "KT(SecureKeyString)";

        public const string AuthSchemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;
    }
}
