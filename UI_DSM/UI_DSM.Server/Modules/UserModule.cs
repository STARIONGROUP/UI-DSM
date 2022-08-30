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
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Carter.ModelBinding;
    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.UserManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="UserModule" /> is a <see cref="ModuleBase" /> to manage
    ///     <see cref="UserEntity" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/User")]
    public class UserModule : EntityModule<UserEntity, UserDto>
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
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

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
        ///     Tries to register a new user base on the provided <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}" /></param>
        /// <param name="dto">The <see cref="RegistrationDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with a <see cref="RegistrationResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public async Task<RegistrationResponseDto> RegisterUser(IUserManager manager, RegistrationDto dto, HttpContext context)
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

            var identityResult = await manager.RegisterUser(user, dto.Password);
            authenticationResponse.IsRequestSuccessful = identityResult.Succeeded;

            if (!identityResult.Succeeded)
            {
                authenticationResponse.Errors = identityResult.Errors;
                context.Response.StatusCode = 422;
                return authenticationResponse;
            }

            authenticationResponse.CreatedUser = (UserDto)identityResult.Entity.ToDto();
            context.Response.StatusCode = 201;
            return authenticationResponse;
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntities(IEntityManager<UserEntity> manager, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="UserEntity" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task GetEntity(IEntityManager<UserEntity> manager, Guid entityId, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            return base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="UserEntity" /> based on its <see cref="UserDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="UserDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task CreateEntity(IEntityManager<UserEntity> manager, UserDto dto, HttpContext context, [FromQuery] int deepLevel = 0)
        {
            await Task.CompletedTask;

            context.Response.StatusCode = 405;

            await context.Response.Negotiate(
                new EntityRequestResponseDto
                {
                    Errors = new List<string>
                    {
                        "Invalid method for creating a user"
                    }
                });
        }

        /// <summary>
        ///     Tries to delete an <see cref="UserEntity" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="UserEntity" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize(Roles = "Administrator")]
        public override Task<RequestResponseDto> DeleteEntity(IEntityManager<UserEntity> manager, Guid entityId, HttpContext context)
        {
            return base.DeleteEntity(manager, entityId, context);
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
