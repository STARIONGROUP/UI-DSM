// --------------------------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;

    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The <see cref="UserController" /> is a <see cref="Controller" /> to manage <see cref="User" />s
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        ///     The <see cref="IConfigurationSection" /> for JWT Authentication
        /// </summary>
        private readonly IConfigurationSection jwtSettings;

        /// <summary>
        ///     The <see cref="UserManager{TUser}" />
        /// </summary>
        private readonly UserManager<User> userManager;

        /// <summary>
        ///     Initialize a new <see cref="UserController" />
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{TUser}" /></param>
        /// <param name="configuration">
        ///     The <see cref="IConfiguration" /> to get the JWT <see cref="IConfigurationSection" />
        /// </param>
        public UserController(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.jwtSettings = configuration.GetSection("JwtSettings");
        }

        /// <summary>
        ///     Tries to log in as a registered user
        /// </summary>
        /// <param name="authentication">The <see cref="AuthenticationDto" /> to use to authenticate</param>
        /// <returns>A <see cref="Task" /></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationDto authentication)
        {
            var user = await this.userManager.FindByNameAsync(authentication.UserName);

            if (user == null || !await this.userManager.CheckPasswordAsync(user, authentication.Password))
            {
                return this.Unauthorized(new AuthenticationResponseDto
                {
                    Errors = new List<string>
                    {
                        "Username and/or password are incorrect"
                    }
                });
            }

            var tokenOptions = this.GenerateTokenOptions(this.GetSigningCredentials(), this.GetClaims(user));
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return this.Ok(new AuthenticationResponseDto { IsRequestSuccessful = true, Token = token });
        }

        /// <summary>
        ///     Tries to register a new user base on the provided <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="registration">The <see cref="RegistrationDto" /></param>
        /// <returns>The <see cref="UserDto" /> if correctly created, a simple <see cref="RegistrationResponseDto" /> otherwise</returns>
        [HttpPost("Register")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationDto registration)
        {
            var user = new User
            {
                UserName = registration.UserName
            };

            var identityResult = await this.userManager.CreateAsync(user, registration.Password);

            var response = new RegistrationResponseDto
            {
                IsRequestSuccessful = identityResult.Succeeded
            };

            if (!identityResult.Succeeded)
            {
                response.Errors = identityResult.Errors.Select(e => e.Description).ToList();
                return this.BadRequest(response);
            }

            var createdUser = await this.userManager.FindByNameAsync(user.UserName);

            response.CreatedUser = createdUser.ToDto();

            return this.StatusCode(201, response);
        }

        /// <summary>
        ///     Get the collections of registered users
        /// </summary>
        /// <returns>A <see cref="Task" /> that will contains the collection of <see cref="UserDto" /></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this.userManager.Users.ToListAsync();

            return this.Ok(users.Select(x => x.ToDto()));
        }

        /// <summary>
        ///     Tries to delete an <see cref="User" /> defined by the given <paramref name="userId" />
        /// </summary>
        /// <param name="userId">The <see cref="User.Id" /></param>
        /// <returns>A <see cref="Task" /> with the result of the delete action</returns>
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound(new RequestResponseDto
                {
                    Errors = new List<string>
                    {
                        $"User with id {userId} not found"
                    }
                });
            }

            if (user.IsAdmin)
            {
                return this.BadRequest(new RegistrationResponseDto
                {
                    Errors = new List<string>
                    {
                        "Forbidden to delete an Administrator"
                    }
                });
            }

            await this.userManager.DeleteAsync(user);

            return this.Ok(new RequestResponseDto
            {
                IsRequestSuccessful = true
            });
        }

        /// <summary>
        ///     Retrieve the <see cref="SigningCredentials" /> based on the <see cref="jwtSettings" />
        /// </summary>
        /// <returns>The <see cref="SigningCredentials" /></returns>
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(this.jwtSettings["securityKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        ///     Assign all <see cref="Claim" /> based on the <see cref="User" />
        /// </summary>
        /// <param name="user">The <see cref="User" /></param>
        /// <returns>A collection of <see cref="Claim" /></returns>
        private IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName)
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }

            return claims;
        }

        /// <summary>
        ///     Generate the <see cref="JwtSecurityToken" /> based for the provided collection of <see cref="Claim" />
        /// </summary>
        /// <param name="signingCredentials">The <see cref="SigningCredentials" /></param>
        /// <param name="claims">The collection of <see cref="Claim" /></param>
        /// <returns>The generated <see cref="JwtSecurityToken" /></returns>
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                this.jwtSettings["validIssuer"],
                this.jwtSettings["validAudience"],
                claims,
                signingCredentials: signingCredentials);

            return tokenOptions;
        }
    }
}
