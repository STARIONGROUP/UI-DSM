// --------------------------------------------------------------------------------------------------------
// <copyright file="ViewProviderService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ViewProviderService
{
    using System.Reflection;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     This service provide <see cref="BaseView" /> and related <see cref="BaseViewViewModel" /> depending on a
    ///     <see cref="View" />
    /// </summary>
    public class ViewProviderService : IViewProviderService
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> that will store a <see cref="View" /> and corresponding
        ///     <see cref="GenericBaseView{TViewModel}" />
        /// </summary>
        private static readonly Dictionary<View, Type> Views = new();

        /// <summary>
        ///     Tries to get the correct <see cref="Type" /> for that correspond for the provided <see cref="View" />
        /// </summary>
        /// <param name="view">The <see cref="View" /></param>
        /// <returns>The retrieves <see cref="Type" /></returns>
        public Type GetViewType(View view)
        {
            return Views.ContainsKey(view) ? Views[view] : null;
        }

        /// <summary>
        ///     Tries to register all <see cref="View" />
        /// </summary>
        public static void RegisterViews()
        {
            var assembly = Assembly.GetAssembly(typeof(GenericBaseView<>));
            var types = assembly?.GetTypes();

            foreach (var viewName in Enum.GetNames<View>())
            {
                var viewComponent = types?.SingleOrDefault(x => x.Name == viewName);

                if (viewComponent != null)
                {
                    Views[Enum.Parse<View>(viewName)] = viewComponent;
                }
            }
        }
    }
}
