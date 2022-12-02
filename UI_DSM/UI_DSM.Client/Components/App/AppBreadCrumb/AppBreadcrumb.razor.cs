// --------------------------------------------------------------------------------------------------------
// <copyright file="AppBreadcrumb.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace AppComponents
{
	using Microsoft.AspNetCore.Components;

	using UI_DSM.Client.Model;

	/// <summary>
	/// Partial class for the component <see cref="AppBreadcrumb"/>
	/// </summary>
	public partial class AppBreadcrumb : IDisposable
	{
		/// <summary>
		/// Gets or sets the Navigation Manager
		/// </summary>
		[Inject]
		public NavigationManager NavigationManager { get; set; }

		/// <summary>
		/// Gets or sets the list of <see cref="BreadcrumbLink"/>
		/// </summary>
		[Parameter]
		public List<BreadcrumbLink> BreadcrumbLinks { get; set; } = new List<BreadcrumbLink>();

		/// <summary>
		/// Gets or sets the names used to identify pages in a Uri
		/// </summary>
		public List<string> Names { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
		{
			base.OnInitialized();

			this.Names = new List<string>()
			{
                "Project",
                "Review",
                "ReviewObjective",
                "ReviewTask"
            };

            this.CreateBreadcrumb(this.NavigationManager.Uri);
            this.NavigationManager.LocationChanged += NavigationManager_LocationChanged;	
		}

		/// <summary>
		/// Event for when the location (url) has changed
		/// </summary>
		/// <param name="sender">the sender of the event</param>
		/// <param name="e">args of the event</param>
		private void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
		{
			this.CreateBreadcrumb(e.Location);
        }

		/// <summary>
		/// Creates a new Breadcrumb overriding the previous one
		/// </summary>
		/// <param name="absoluteUri">the absolute uri used to create the breadcrumb</param>
		private void CreateBreadcrumb(string absoluteUri)
		{
            this.BreadcrumbLinks.Clear();
            this.BreadcrumbLinks = this.CreateAbsoluteLinks(absoluteUri).ToList();
            this.StateHasChanged();
        }

		/// <summary>
		/// Creates a Breadcrumb for the specified <paramref name="absoluteUri"/>
		/// </summary>
		/// <param name="absoluteUri">the absolute Uri used to create the links</param>
		/// <returns>an <see cref="IEnumerable{BreadcrumbLink}"/></returns>
		private IEnumerable<BreadcrumbLink> CreateAbsoluteLinks(string absoluteUri)
		{
            var segments = absoluteUri.Split('/');
			var links = new List<BreadcrumbLink>();

			var title = "Home";
            for (int i = 0; i < segments.Length; i++)
			{
                var segment = segments[i];
				if (this.Names.Contains(segment))
				{
                    var lastIndex = absoluteUri.IndexOf(segment);
                    links.Add(new BreadcrumbLink(title, absoluteUri.Substring(0, lastIndex)));
					title = segment;
                }
            }

            links.Add(new BreadcrumbLink(title, absoluteUri));

            return links;
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }
    }
}
