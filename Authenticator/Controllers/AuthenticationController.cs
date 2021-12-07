using AuthenticationService.Core.Requests;
using AuthenticationService.Models;
using AuthenticationService.Services.Access;
using AuthenticationService.Services.Refresh;
using AuthenticationService.Services.Repositories;
using AuthenticationService.Services.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IAccessTokenGenerator accessTokenGenerator;
        private readonly IRefreshTokenGenerator refreshTokenGenerator;
        private readonly ITokenRepository tokenRepository;
        private readonly IRefreshTokenValidator refreshTokenValidator;
        private readonly ILogger<AuthenticationController> logger;
        private readonly PasswordHasher<string> passwordHasher;

        public AuthenticationController(
            IUserRepository userRepository,
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            ITokenRepository tokenRepository,
            IRefreshTokenValidator refreshTokenValidator,
            ILogger<AuthenticationController> logger)
        {
            this.userRepository = userRepository;
            this.accessTokenGenerator = accessTokenGenerator;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.tokenRepository = tokenRepository;
            this.refreshTokenValidator = refreshTokenValidator;
            this.logger = logger;
            passwordHasher = new();
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
                return BadRequest(new
                {
                    message = "Passwords are not match"
                });

            if (await userRepository.FindByName(registerRequest.Username) != null)
                return Conflict(new
                {
                    message = "A user with this name already exists"
                });

            Person person = new()
            {
                Username = registerRequest.Username,
                Password = passwordHasher.HashPassword(registerRequest.Username, registerRequest.Password),
                Role = PersonRole.Student
            };

            await userRepository.Add(person);
            return Ok();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            Person person = await AuthorizedPersonOrDefault(loginRequest.Login, loginRequest.Password);

            if (person == null)
                return Unauthorized(new
                {
                    mesage = "Wrong username and/or password"
                });

            string accessToken = accessTokenGenerator.GenerateToken(person);
            string refreshToken = refreshTokenGenerator.GenerateToken();
            await tokenRepository.Add(refreshToken, person);

            logger.LogInformation($"succesful login for {person.Username}");

            return Ok(new
            {
                accessToken,
                refreshToken,
                role = person.Role.ToString()
            });
        }

        [Authorize]
        [HttpGet]
        [Route("/username")]
        public IActionResult Username()
        {
            return Ok(new
            {
                username = User.Claims.First(claim => claim.Type == ClaimTypes.Name).Value
            });
        }

        [HttpPost]
        [Route("/refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            JwtToken token = await RefreshJwtTokenOrDefault(refreshRequest.RefreshToken);

            if (token == null)
                return Unauthorized(new
                {
                    message = "Not valid refresh token"
                });

            string accessToken = accessTokenGenerator.GenerateToken(token.Owner);

            return Ok(new
            {
                accessToken
            });
        }

        [HttpPost]
        [Route("/logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
        {
            JwtToken token = await RefreshJwtTokenOrDefault(logoutRequest.RefreshToken);

            if (token == null)
                return Unauthorized(new
                {
                    message = "Not valid refresh token"
                });

            await tokenRepository.Remove(token);

            logger.LogInformation($"succesful logout for {token.Owner.Username}");

            return Ok(new
            {
                message = "logout succesful"
            });
        }

        [HttpPost]
        [Route("/changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest passwordChangeRequest)
        {
            if (passwordChangeRequest.NewPassword != passwordChangeRequest.ConfirmPassword)
                return BadRequest(new
                {
                    message = "New password and confirmed password aren't the same"
                });

            Person person = await AuthorizedPersonOrDefault(passwordChangeRequest.Username, passwordChangeRequest.Password);

            if (person == null)
                return Unauthorized(new
                {
                    mesage = "Wrong username and/or password"
                });

            person.Password = passwordHasher.HashPassword(person.Username, passwordChangeRequest.NewPassword);
            person = await userRepository.Update(person);
            await tokenRepository.RemoveByOwner(person);

            return Ok(new
            {
                message = "Password was succesfully changed. All your sessions have been removed"
            });
        }

        protected async Task<JwtToken> RefreshJwtTokenOrDefault(string refreshToken)
        {
            if (!refreshTokenValidator.IsValid(refreshToken))
                return null;

            JwtToken token = await tokenRepository.Find(refreshToken);
            return token;
        }

        protected async Task<Person> AuthorizedPersonOrDefault(string username, string password)
        {
            Person person = await userRepository.FindByName(username);

            if (person == null)
                return null;

            if (passwordHasher.VerifyHashedPassword(person.Username, person.Password, password) == PasswordVerificationResult.Failed)
                return null;

            return person;
        }
    }
}
