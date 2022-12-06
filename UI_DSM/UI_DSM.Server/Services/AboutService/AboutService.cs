// --------------------------------------------------------------------------------------------------------
// <copyright file="AboutService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.AboutService
{
    using System.Reflection;

    using UI_DSM.Server.Context;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     This service provides information related to the current status of the application (uptime, DLL version,...)
    /// </summary>
    public class AboutService : IAboutService
    {
        /// <summary>
        ///     An array of assembly names that have a huge interest to get information of
        /// </summary>
        private readonly string[] assembliesWithInterest =
        {
            "UI_DSM",
            "CDP4"
        };

        /// <summary>
        ///     The <see cref="DateTime" /> where the server started
        /// </summary>
        private readonly DateTime startTime;

        /// <summary>
        ///     A collection of <see cref="AssemblyInformationDto" />
        /// </summary>
        private List<AssemblyInformationDto> assembliesInformation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AboutService" /> class.
        /// </summary>
        public AboutService(DateTime applicationStartTime)
        {
            this.startTime = applicationStartTime;
            this.RetrieveAssembliesInformation();
        }

        /// <summary>
        ///     Gets this system information
        /// </summary>
        /// <param name="databaseContext">The <see cref="DatabaseContext" /></param>
        /// <returns>The <see cref="SystemInformationDto" /></returns>
        public SystemInformationDto GetSystemInformation(DatabaseContext databaseContext)
        {
            return new SystemInformationDto
            {
                IsAlive = true,
                StartTime = this.startTime,
                AssembliesInformation = this.assembliesInformation,
                DataBaseInformation = databaseContext.GetDatabaseInformation()
            };
        }

        /// <summary>
        ///     Gets information relatives to <see cref="Assembly" /> that are interesting to get
        /// </summary>
        private void RetrieveAssembliesInformation()
        {
            this.assembliesInformation = new List<AssemblyInformationDto>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assemblyName in this.assembliesWithInterest)
            {
                var filteredAssemblies = assemblies.Where(x => x.FullName != null && x.FullName.Contains(assemblyName));

                foreach (var assembly in filteredAssemblies)
                {
                    var assemblyInformation = assembly.GetName();
                    this.assembliesInformation.Add(new AssemblyInformationDto { AssemblyName = assemblyInformation.Name, AssemblyVersion = assemblyInformation.Version });
                }
            }
        }
    }
}
