// --------------------------------------------------------------------------------------------------------
// <copyright file="AppBreadcrumb.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.AppBreadCrumb
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Routing;

    using UI_DSM.Client.Services.RouteParserService;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Component that will provide information related to the current uri
    /// </summary>
    public partial class AppBreadcrumb : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="ParsedUrlDto" />
        /// </summary>
        private List<ParsedUrlDto> parsedUrl = new();

        /// <summary>
        ///     Gets or sets the Navigation Manager
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     The <see cref="IRouteParserService" />
        /// </summary>
        [Inject]
        public IRouteParserService RouteParser { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.NavigationManager.LocationChanged -= this.NavigationManager_LocationChanged;
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.NavigationManager.LocationChanged += this.NavigationManager_LocationChanged;
            this.NavigationManager_LocationChanged(this, new LocationChangedEventArgs(this.NavigationManager.Uri, false));
        }

        /// <summary>
        ///     Handle the changed of the uri
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="args">The <see cref="LocationChangeException" /></param>
        private void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs args)
        {
            this.InvokeAsync(() => this.ParseRoute($"/{args.Location.Replace(this.NavigationManager.BaseUri, string.Empty)}"));
        }

        /// <summary>
        ///     Parses the new route
        /// </summary>
        /// <param name="routeToParse">The route to parse</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task ParseRoute(string routeToParse)
        {
            this.parsedUrl.Clear();
            this.parsedUrl.AddRange(await this.RouteParser.ParseUrl(routeToParse));
            await this.InvokeAsync(this.StateHasChanged);
        }
    }
}
