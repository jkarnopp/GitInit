using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.Services;
using GitInitTest.Site.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GitInitTest.Site.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _logger = logger;
        }
        [HttpPost]
        //[Route("login")]
        public async Task<IActionResult> Create([FromBody] JwtIdentityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (signInResult.Succeeded)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfigConstants.Key));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,model.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, model.UserName)
                    };

                    var token = new JwtSecurityToken(
                       JwtConfigConstants.Issuer,
                       JwtConfigConstants.Audience,
                       claims,
                       expires: DateTime.UtcNow.AddMinutes(30),
                       signingCredentials: creds);

                    var results = new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    };

                    return Created("", results);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!await _accountService.IsValidUserCredentials(request))
            {
                return Unauthorized();
            }

            //var role = _userService.GetUserRole(request.UserName);
            //var claims = new[]
            //{
            //    new Claim(ClaimTypes.Name,request.UserName),
            //    new Claim(ClaimTypes.Role, role)
            //};

            //var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims, DateTime.Now);
            //_logger.LogInformation($"User [{request.UserName}] logged in the system.");
            //return Ok(new LoginResult
            //{
            //    UserName = request.UserName,
            //    Role = role,
            //    AccessToken = jwtResult.AccessToken,
            //    RefreshToken = jwtResult.RefreshToken.TokenString
            //});

            return Ok(await _accountService.Login(request));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            //var user = new ApplicationUser
            //{
            //    UserName = registration.UserName,
            //    Email = registration.Email,
            //    FirstName = registration.FirstName,
            //    LastName = registration.LastName,
            //    PhoneNumber = registration.PhoneNumber,
            //    EmailConfirmed = true
            //};
            //var result = await _userManager.CreateAsync(user, registration.Password);
            var result = await _accountService.Register(registration);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var request = new LoginRequestDto(registration.UserName, registration.Password);
                return Ok(await _accountService.Login(request));


                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Page(
                //    "/Account/ConfirmEmail",
                //    pageHandler: null,
                //    values: new {userId = user.Id, code = code},
                //    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                //await _signInManager.SignInAsync(user, isPersistent: false);
                //return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest();


        }


        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResultDto
            {
                UserName = User.Identity.Name,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                //Roles = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                OriginalUserName = User.FindFirst("OriginalUserName")?.Value
            });
        }


        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        public ActionResult Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity.Name;
            _accountService.Logout(userName);
            //_jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            //_logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }



        [HttpPost("refresh-token")]
        [Authorize(AuthenticationSchemes = JwtConfigConstants.AuthSchemes)]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var userName = User.Identity.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");


                //var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                var jwtResult = _accountService.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResultDto
                {
                    UserName = userName,
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray(),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }


        /*
                [HttpPost("impersonation")]
                [Authorize(Roles = "Admin")]
                public ActionResult Impersonate([FromBody] ImpersonationRequestDto request)
                {
                    var userName = User.Identity.Name;
                    _logger.LogInformation($"User [{userName}] is trying to impersonate [{request.UserName}].");
        
                    var impersonatedRole = _userService.GetUserRole(request.UserName);
                    if (string.IsNullOrWhiteSpace(impersonatedRole))
                    {
                        _logger.LogInformation($"User [{userName}] failed to impersonate [{request.UserName}] due to the target user not found.");
                        return BadRequest($"The target user [{request.UserName}] is not found.");
                    }
                    if (impersonatedRole == UserRoles.Admin)
                    {
                        _logger.LogInformation($"User [{userName}] is not allowed to impersonate another Admin.");
                        return BadRequest("This action is not supported.");
                    }
        
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name,request.UserName),
                        new Claim(ClaimTypes.Role, impersonatedRole),
                        new Claim("OriginalUserName", userName)
                    };
        
                    var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims, DateTime.Now);
                    _logger.LogInformation($"User [{request.UserName}] is impersonating [{request.UserName}] in the system.");
                    return Ok(new LoginResult
                    {
                        UserName = request.UserName,
                        Role = impersonatedRole,
                        OriginalUserName = userName,
                        AccessToken = jwtResult.AccessToken,
                        RefreshToken = jwtResult.RefreshToken.TokenString
                    });
                }
        
                [HttpPost("stop-impersonation")]
                public ActionResult StopImpersonation()
                {
                    var userName = User.Identity.Name;
                    var originalUserName = User.FindFirst("OriginalUserName")?.Value;
                    if (string.IsNullOrWhiteSpace(originalUserName))
                    {
                        return BadRequest("You are not impersonating anyone.");
                    }
                    _logger.LogInformation($"User [{originalUserName}] is trying to stop impersonate [{userName}].");
        
                    var role = _userService.GetUserRole(originalUserName);
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name,originalUserName),
                        new Claim(ClaimTypes.Role, role)
                    };
        
                    var jwtResult = _jwtAuthManager.GenerateTokens(originalUserName, claims, DateTime.Now);
                    _logger.LogInformation($"User [{originalUserName}] has stopped impersonation.");
                    return Ok(new LoginResult
                    {
                        UserName = originalUserName,
                        Role = role,
                        OriginalUserName = null,
                        AccessToken = jwtResult.AccessToken,
                        RefreshToken = jwtResult.RefreshToken.TokenString
                    });
                }*/
    }
}

