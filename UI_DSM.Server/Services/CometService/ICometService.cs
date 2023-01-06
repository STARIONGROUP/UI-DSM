// --------------------------------------------------------------------------------------------------------
// <copyright file="ICometService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.CometService
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Shared.DTO.CometData;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="CometService" />
    /// </summary>
    public interface ICometService
    {
        /// <summary>
        ///     Tries to login to a Comet <see cref="ISession" />
        /// </summary>
        /// <param name="uploadData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the login process</returns>
        Task<Tuple<AuthenticationStatus, Guid>> Login(CometAuthenticationData uploadData);

        /// <summary>
        ///     Closes a <see cref="ISession" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /> to close</param>
        /// <returns>A <see cref="Task" /></returns>
        Task Close(Guid sessionId);

        /// <summary>
        ///     Gets a collection of available <see cref="EngineeringModelSetup" />
        /// </summary>
        /// <param name="sessionId">
        ///     The <see cref="Guid" /> of the <see cref="ISession" /> to get
        ///     <see cref="EngineeringModelSetup" />
        /// </param>
        /// <returns>A collection of <see cref="EngineeringModelSetup" /></returns>
        IEnumerable<EngineeringModelSetup> GetAvailableEngineeringModel(Guid sessionId);

        /// <summary>
        ///     Reads an <see cref="IterationSetup" /> to get the corresponding
        ///     <see cref="Iteration" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /></param>
        /// <param name="iterationSetup">The <see cref="IterationSetup" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Iteration" /></returns>
        Task<Iteration> ReadIteration(Guid sessionId, IterationSetup iterationSetup);

        /// <summary>
        ///     Creates an Annex C3 file for the provided iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>A <see cref="Task" /> with the name of the created file</returns>
        Task<string> CreateAnnexC3File(Iteration iteration);

        /// <summary>
        ///     Gets an <see cref="IterationSetup" /> based on provided <see cref="Guid" />s
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /></param>
        /// <param name="modelId">The <see cref="Guid" /> of the <see cref="EngineeringModelSetup" /></param>
        /// <param name="iterationId">The <see cref="Guid" /> of the <see cref="IterationSetup" /></param>
        /// <returns>The <see cref="IterationSetup" /></returns>
        IterationSetup GetIterationSetup(Guid sessionId, Guid modelId, Guid iterationId);

        /// <summary>
        ///     Tries to read an Annex C3 file
        /// </summary>
        /// <param name="file">The <see cref="IFormFile" /></param>
        /// <param name="authenticationData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the operation</returns>
        Task<Tuple<AuthenticationStatus, Guid>> ReadAnnexC3File(IFormFile file, CometAuthenticationData authenticationData);

        /// <summary>
        ///     Gets the <see cref="Iteration" /> contained into a <see cref="Model" />
        /// </summary>
        /// <param name="model">The <see cref="Model" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Iteration" /></returns>
        Task<Iteration> GetIteration(Model model);

        /// <summary>
        ///     Gets all <see cref="DomainOfExpertise" /> that are contained inside a Model
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>The collection of <see cref="DomainOfExpertise" /></returns>
        IEnumerable<DomainOfExpertise> GetDomainOfExpertises(Iteration iteration);
    }
}
