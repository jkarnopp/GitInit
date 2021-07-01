using GitInitTest.Entities.Dtos;
using GitInitTest.Entities.Models;
using GitInitTest.Site.Controllers.api;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GitInitTest.Site.Services
{
    public interface IAccountService
    {
        Task<bool> IsValidUserCredentials(LoginRequestDto loginRequestDto);
        Task<LoginResultDto> Login(LoginRequestDto loginRequestDto);
        Task<IdentityResult> Register(RegistrationDto registration);
        void Logout(string? userName);
        JwtAuthResultDto Refresh(string requestRefreshToken, string accessToken, in DateTime now);
    }
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtAuthService jwtAuthService, ILogger<AccountService> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtAuthService = jwtAuthService;
            _logger = logger;
        }
        public async Task<bool> IsValidUserCredentials(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);

            return signInResult.Succeeded;
        }

        public async Task<LoginResultDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByNameAsync(loginRequestDto.UserName);
            //var role = _userService.GetUserRole(request.UserName);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, loginRequestDto.UserName));

            if (roles != null)
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

            //var claims = new[]
            //{
            //    new Claim(ClaimTypes.Name,loginRequestDto.UserName),
            //    new Claim(ClaimTypes.Role, roles.ToString())
            //};


            var jwtResult = _jwtAuthService.GenerateTokens(loginRequestDto.UserName, claims.ToArray(), DateTime.Now);
            _logger.LogInformation($"User [{loginRequestDto.UserName}] logged in the system.");
            return new LoginResultDto
            {
                UserName = loginRequestDto.UserName,
                Roles = roles?.ToArray(),
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            };
        }

        public async Task<IdentityResult> Register(RegistrationDto registration)
        {
            var user = new ApplicationUser
            {
                UserName = registration.UserName,
                Email = registration.Email,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PhoneNumber = registration.PhoneNumber,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, registration.Password);

            return result;
        }

        public void Logout(string? userName)
        {
            _jwtAuthService.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
        }

        public JwtAuthResultDto Refresh(string requestRefreshToken, string accessToken, in DateTime now)
        {
            return _jwtAuthService.Refresh(requestRefreshToken, accessToken, DateTime.Now);
        }
    }
}
