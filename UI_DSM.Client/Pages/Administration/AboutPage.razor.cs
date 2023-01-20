// --------------------------------------------------------------------------------------------------------
// <copyright file="AboutPage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.Administration
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.AboutService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Page that will get information related to the system
    /// </summary>
    public partial class AboutPage
    {
        /// <summary>
        ///     The <see cref="SystemInformationDto" />
        /// </summary>
        private SystemInformationDto systemInformation;

        /// <summary>
        ///     Value indicating if the current page is loading
        /// </summary>
        public bool IsLoading { get; set; } = true;

        /// <summary>
        ///     The <see cref="IAboutService" />
        /// </summary>
        [Inject]
        public IAboutService AboutService { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            this.systemInformation = await this.AboutService.GetSystemInformation();
            this.IsLoading = false;
            await base.OnInitializedAsync();
        }
    }
}
