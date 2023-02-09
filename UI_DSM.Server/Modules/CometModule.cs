// --------------------------------------------------------------------------------------------------------
// <copyright file="CometModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
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
    using System.Text.Json;

    using Carter.ModelBinding;
    using Carter.Response;

    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Server.Services.CometService;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     The <see cref="CometModule" /> is a module that handles a connection to a Comet server
    ///     to be able to download an <see cref="Iteration" />
    /// </summary>
    [Microsoft.AspNetCore.Components.Route("api/Comet")]
    public class CometModule : ModuleBase
    {
        /// <summary>
        ///     The <see cref="ICometService" />
        /// </summary>
        private readonly ICometService cometService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CometModule" /> class.
        /// </summary>
        /// <param name="cometService">The <see cref="ICometService" /></param>
        public CometModule(ICometService cometService)
        {
            this.cometService = cometService;
        }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost($"{this.MainRoute}/Login", this.Login)
                .Accepts<CometAuthenticationData>("application/json")
                .Produces<CometAuthenticationResponse>()
                .Produces<CometAuthenticationResponse>(400)
                .Produces<CometAuthenticationResponse>(401)
                .Produces<CometAuthenticationResponse>(500)
                .WithTags("Comet")
                .WithName("Comet/Login");

            app.MapGet(this.MainRoute + "/{sessionId:guid}/Models", this.GetAvailableEngineeringModels)
                .Produces<ModelsDataResponse>()
                .Produces<ModelsDataResponse>(404)
                .WithTags("Comet")
                .WithName("Comet/Models");

            app.MapPost(this.MainRoute + "/{sessionId:guid}/Models/Upload", this.UploadIteration)
                .Accepts<ModelUploadData>("application/json")
                .Produces<ModelUploadResponse>()
                .Produces<ModelUploadResponse>(400)
                .WithTags("Comet")
                .WithName("Comet/Upload");

            app.MapDelete(this.MainRoute + "/{sessionId:guid}", this.Logout)
                .Produces<RequestResponseDto>()
                .Produces<RequestResponseDto>(404)
                .WithTags("Comet")
                .WithName("Comet/Logout");

            app.MapPost($"{this.MainRoute}/Upload", this.UploadAnnexC3);
        }

        /// <summary>
        ///     Uploads an Annex C3 file and tries to open it
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task UploadAnnexC3(HttpContext context)
        {
            var response = new CometAuthenticationResponse();

            var authenticationDataValue = VerifyFormRequestAndExtractData(context, response);

            if (context.Response.StatusCode != 200)
            {
                await context.Response.Negotiate(response);
                return;
            }

            CometAuthenticationData authenticationData;

            try
            {
                authenticationData = JsonSerializer.Deserialize<CometAuthenticationData>(authenticationDataValue);
                var validation = context.Request.Validate(authenticationData);

                if (!validation.IsValid)
                {
                    throw new InvalidDataException();
                }
            }
            catch
            {
                response.Errors = new List<string>
                {
                    "The field 'authenticationData' is invalid!"
                };

                context.Response.StatusCode = 400;
                await context.Response.Negotiate(response);
                return;
            }

            var file = context.Request.Form.Files[0];
            var result = await this.cometService.ReadAnnexC3File(file, authenticationData);

            switch (result.Item1)
            {
                case AuthenticationStatus.Success:
                    response.SessionId = result.Item2;
                    response.IsRequestSuccessful = true;
                    break;
                case AuthenticationStatus.Fail:
                    response.Errors = new List<string> { "Invalid username/password" };
                    context.Response.StatusCode = 401;
                    break;
                case AuthenticationStatus.ServerFailure:
                    response.Errors = new List<string> { "Invalid AnnexC3 file" };
                    context.Response.StatusCode = 500;
                    break;
            }

            await context.Response.Negotiate(response);
        }

        /// <summary>
        ///     Close the <see cref="CDP4Dal.ISession" /> at the given <see cref="Guid" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="CDP4Dal.ISession" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task Logout(Guid sessionId, HttpContext context)
        {
            var response = new RequestResponseDto();

            try
            {
                await this.cometService.Close(sessionId);
                response.IsRequestSuccessful = true;
            }
            catch (Exception exception)
            {
                response.Errors = new List<string> { exception.Message };
                context.Response.StatusCode = 404;
            }

            await context.Response.Negotiate(response);
        }

        /// <summary>
        ///     Gets all <see cref="EngineeringModel" /> available inside a <see cref="CDP4Dal.ISession" /> that contains frozen
        ///     <see cref="Iteration" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="CDP4Dal.ISession" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetAvailableEngineeringModels(Guid sessionId, HttpContext context)
        {
            var response = new ModelsDataResponse();

            try
            {
                var models = this.cometService.GetAvailableEngineeringModel(sessionId);

                foreach (var model in models)
                {
                    var frozenIteration = model.IterationSetup.Where(iteration => !iteration.IsDeleted && iteration.FrozenOn != null);
                    
                    response.AvailableModels.Add(new EngineeringModelData()
                    {
                        EngineeringModelName = model.Name,
                        EngineeringId = model.Iid,
                        Iterations = frozenIteration.Select(x =>
                            new IterationData()
                            {
                                IterationId = x.IterationIid,
                                IterationName = $"Iteration {x.IterationNumber} - {x.FrozenOn}"
                            }).ToList()
                    });
                }

                response.IsRequestSuccessful = true;
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 404;
                response.Errors = new List<string> { exception.Message };
            }

            await context.Response.Negotiate(response);
        }

        /// <summary>
        ///     Tries to log to a Comet session
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /> to use to establish the connection</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task Login([FromBody] CometAuthenticationData uploadData, HttpContext context)
        {
            var cometResponse = new CometAuthenticationResponse();
            var validationResult = context.Request.Validate(uploadData);

            if (!validationResult.IsValid)
            {
                cometResponse.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 400;
                await context.Response.Negotiate(cometResponse);
                return;
            }

            var result = await this.cometService.Login(uploadData);

            switch (result.Item1)
            {
                case AuthenticationStatus.Success:
                    cometResponse.SessionId = result.Item2;
                    cometResponse.IsRequestSuccessful = true;
                    break;
                case AuthenticationStatus.Fail:
                    cometResponse.Errors = new List<string> { "Invalid username/password" };
                    context.Response.StatusCode = 401;
                    break;
                case AuthenticationStatus.ServerFailure:
                    cometResponse.Errors = new List<string> { $"Comet server not reachable at the folling url {uploadData.Url}" };
                    context.Response.StatusCode = 500;
                    break;
            }

            await context.Response.Negotiate(cometResponse);
        }

        /// <summary>
        ///     Saves the selected <see cref="Iteration" /> into an Annex C3 file
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="CDP4Dal.ISession" /></param>
        /// <param name="modelUpload">The <see cref="ModelUploadData" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task UploadIteration(Guid sessionId, [FromBody] ModelUploadData modelUpload, HttpContext context)
        {
            var response = new ModelUploadResponse();

            var validationResult = context.Request.Validate(modelUpload);

            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                context.Response.StatusCode = 400;
                await context.Response.Negotiate(response);
                return;
            }

            try
            {
                var iterationSetup = this.cometService.GetIterationSetup(sessionId, modelUpload.ModelId, modelUpload.IterationId);
                var iteration = await this.cometService.ReadIteration(sessionId, iterationSetup);
                var annexC3File = await this.cometService.CreateAnnexC3File(iteration);
                response.UploadedFilePath = annexC3File;
                response.IsRequestSuccessful = true;
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = 400;
                response.Errors = new List<string> { exception.Message };
            }

            await context.Response.Negotiate(response);
        }

        /// <summary>
        ///     Verifies that the <see cref="IFormCollection" /> contains <see cref="IFormFile" /> and <see cref="StringValues" />
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="response">The <see cref="RequestResponseDto" /></param>
        /// <returns>
        ///     The contained value inside the <see cref="IFormCollection" /> for the <see cref="CometAuthenticationData" /> field
        /// </returns>
        private static string VerifyFormRequestAndExtractData(HttpContext context, RequestResponseDto response)
        {
            if (context.Request.Form.Files.Count != 1 || context.Request.Form.Files[0].ContentType != "application/x-zip-compressed")
            {
                response.Errors = new List<string>
                {
                    "The request only accept a single zip file!"
                };

                context.Response.StatusCode = 400;
                return string.Empty;
            }

            if (!context.Request.Form.TryGetValue("authenticationData", out var authenticationDataValue))
            {
                response.Errors = new List<string>
                {
                    "The request must contains a field 'authenticationData'!"
                };

                context.Response.StatusCode = 400;
                return string.Empty;
            }

            return authenticationDataValue.First();
        }
    }
}
