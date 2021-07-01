using GitInitTest.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitInitTest.Site.Controllers.api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public List<string> GetValues()
        {
            return new List<string>() { "Value1", "Value2" };
        }
    }
}
