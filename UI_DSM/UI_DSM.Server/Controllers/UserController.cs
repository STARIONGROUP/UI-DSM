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

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    using UI_DSM.Server.Models;
    using UI_DSM.Shared.DTO;

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
                    ErrorMessage = "Username and/or password are incorrect"
                });
            }

            var tokenOptions = this.GenerateTokenOptions(this.GetSigningCredentials(), this.GetClaims(user));
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return this.Ok(new AuthenticationResponseDto { IsAuthenticated = true, Token = token });
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
