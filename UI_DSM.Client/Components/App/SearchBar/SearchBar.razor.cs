// --------------------------------------------------------------------------------------------------------
// <copyright file="SearchBar.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.App.SearchBar
{
    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     The searchBar enables to proceed to a search query
    /// </summary>
    public partial class SearchBar
    {
        /// <summary>
        ///     The keyword
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        ///     The <see cref="NavigationManager" />
        /// </summary>
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        /// <summary>
        ///     Proceed to the search query
        /// </summary>
        private void Search()
        {
            if (string.IsNullOrEmpty(this.Keyword))
            {
                return;
            }

            this.NavigationManager.NavigateTo($"Search/{this.Keyword}");
            this.Keyword = string.Empty;
        }
    }
}
