// --------------------------------------------------------------------------------------------------------
// <copyright file="App.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nathanael Smiechowski
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.SearchDto.Generator
{
    using GP.SearchService.SearchDtoGenerator;
    using UI_DSM.Shared.Models;
    using System.Threading.Tasks;

    using System.Reflection;

    /// <summary>
    /// The <see cref="App"/> is the entry point for the search dto generator
    /// </summary>
    public class App
    {
        /// <summary>
        /// Main method, starts the generator
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>A <see cref="Task"/></returns>
        public static async Task Main(string[] arguments)
        {
            await SearchDtoGenerator.Start(Assembly.GetAssembly(typeof(Comment)), arguments);
        }
    }
}
