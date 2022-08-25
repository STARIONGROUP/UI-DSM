// --------------------------------------------------------------------------------------------------------
// <copyright file="UserModule.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Modules
{
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Carter.ModelBinding;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ProjectModule" /> is a <see cref="ModuleBase" /> to manage
    ///     <see cref="User" />
    /// </summary>
    [Route("api/User")]
    public class UserModule : ModuleBase
    {
        /// <summary>
        ///     The <see cref="IConfigurationSection" /> for JWT Authentication
        /// </summary>
        private readonly IConfigurationSection jwtSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserModule" /> class.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration" /></param>
        public UserModule(IConfiguration configuration)
        {
            this.jwtSettings = configuration.GetSection("JwtSettings");
        }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost($"{this.MainRoute}/Login", this.Login)
                .Accepts<AuthenticationDto>("application/json")
                .Produces<AuthenticationResponseDto>()
                .Produces<AuthenticationResponseDto>(422)
                .Produces<AuthenticationResponseDto>(401)
                .WithTags("User")
                .WithName("User/Login");

            app.MapPost($"{this.MainRoute}/Register", this.RegisterUser)
                .Accepts<RegistrationDto>("application/json")
                .Produces<RegistrationResponseDto>(201)
                .Produces<RegistrationResponseDto>(422)
                .WithTags("User")
                .WithName("User/Register");

            app.MapGet(this.MainRoute, this.GetUsers)
                .Produces<IEnumerable<UserDto>>()
                .WithTags("User")
                .WithName("User/GetUsers");

            app.MapDelete(this.MainRoute + "/{userId}", this.DeleteUser)
                .Produces<RequestResponseDto>()
                .Produces<RequestResponseDto>(403)
                .Produces<RequestResponseDto>(404)
                .Produces<RequestResponseDto>(500)
                .WithTags("User")
                .WithName("User/Delete");
        }

        /// <summary>
        ///     Tries to register a new user base on the provided <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}" /></param>
        /// <param name="dto">The <see cref="RegistrationDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with a <see cref="RegistrationResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public async Task<RegistrationResponseDto> RegisterUser(UserManager<User> manager, RegistrationDto dto, HttpContext context)
        {
            var validationResult = context.Request.Validate(dto);
            var authenticationResponse = new RegistrationResponseDto();

            if (!validationResult.IsValid)
            {
                authenticationResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 422;
                return authenticationResponse;
            }

            var user = new User
                { UserName = dto.UserName };

            var identityResult = await manager.CreateAsync(user, dto.Password);
            authenticationResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                authenticationResponse.Errors = identityResult.Errors.Select(e => e.Description).ToList();
                context.Response.StatusCode = 422;
                return authenticationResponse;
            }

            var createdUser = await manager.FindByNameAsync(user.UserName);
            authenticationResponse.CreatedUser = createdUser.ToDto();
            context.Response.StatusCode = 201;
            return authenticationResponse;
        }

        /// <summary>
        ///     Tries to log in as a registered user
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}" /></param>
        /// <param name="dto">The <see cref="AuthenticationDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="AuthenticationResponseDto" /> as result</returns>
        public async Task<AuthenticationResponseDto> Login(UserManager<User> manager, AuthenticationDto dto, HttpContext context)
        {
            var validationResult = context.Request.Validate(dto);
            var authenticationResponse = new AuthenticationResponseDto();

            if (!validationResult.IsValid)
            {
                authenticationResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 422;
                return authenticationResponse;
            }

            var user = await manager.FindByNameAsync(dto.UserName);

            if (user == null || !await manager.CheckPasswordAsync(user, dto.Password))
            {
                authenticationResponse.Errors = new List<string>
                {
                    "Username and/or password are incorrect"
                };

                context.Response.StatusCode = 401;
                return authenticationResponse;
            }

            var tokenOptions = this.GenerateTokenOptions(this.GetSigningCredentials(), this.GetClaims(user));
            authenticationResponse.Token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            authenticationResponse.IsRequestSuccessful = true;
            return authenticationResponse;
        }

        /// <summary>
        ///     Get the collections of registered users
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="UserDto" /></returns>
        [Authorize(Roles = "Administrator")]
        public async Task<IEnumerable<UserDto>> GetUsers(UserManager<User> manager)
        {
            var users = await manager.Users.ToListAsync();
            return users.Select(x => x.ToDto());
        }

        /// <summary>
        ///     Tries to delete an <see cref="User" /> defined by the given <paramref name="userId" />
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}" /></param>
        /// <param name="userId">The <see cref="User.Id" /></param>
        /// <param name="response">The <see cref="HttpResponse" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public async Task<RequestResponseDto> DeleteUser(UserManager<User> manager, string userId, HttpResponse response)
        {
            var user = await manager.FindByIdAsync(userId);
            var requestResponse = new RequestResponseDto();

            if (user == null)
            {
                response.StatusCode = 404;

                requestResponse.Errors = new List<string>
                {
                    $"User with id {userId} not found"
                };

                return requestResponse;
            }

            if (user.IsAdmin)
            {
                response.StatusCode = 403;

                requestResponse.Errors = new List<string>
                {
                    "Forbidden to delete an Administrator"
                };

                return requestResponse;
            }

            var identityResult = await manager.DeleteAsync(user);

            requestResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                response.StatusCode = 500;
                requestResponse.Errors = identityResult.Errors.Select(x => x.Description).ToList();
            }

            return requestResponse;
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
