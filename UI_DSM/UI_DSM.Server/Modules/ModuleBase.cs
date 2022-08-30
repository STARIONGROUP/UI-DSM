// --------------------------------------------------------------------------------------------------------
// <copyright file="ModuleBase.cs" company="RHEA System S.A.">
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
    using Carter;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services;

    /// <summary>
    ///     Base class for all <see cref="ICarterModule" /> for this REST Api
    /// </summary>
    public abstract class ModuleBase : ICarterModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        protected ModuleBase()
        {
            this.MainRoute = this.GetRoute();
        }

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> that store the main route for all <see cref="ServiceBase" />
        /// </summary>
        private static Dictionary<Type, string> ModuleRoute { get; } = new();

        /// <summary>
        ///     Main Route for the <see cref="ServiceBase" />
        /// </summary>
        protected string MainRoute { get; private set; }

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        public abstract void AddRoutes(IEndpointRouteBuilder app);

        /// <summary>
        ///     Register a <see cref="Type" /> if this <see cref="Type" /> derives from <see cref="ModuleBase" />
        /// </summary>
        public static void RegisterModule<T>()
        {
            RegisterModule(typeof(T));
        }

        /// <summary>
        ///     Register a <see cref="Type" /> if this <see cref="Type" /> derives from <see cref="ModuleBase" />
        /// </summary>
        /// <param name="moduleType">The <see cref="Type" /> to register</param>
        public static void RegisterModule(Type moduleType)
        {
            if (!moduleType.IsSubclassOf(typeof(ModuleBase)))
            {
                return;
            }

            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(moduleType, typeof(RouteAttribute));

            if (routeAttribute == null)
            {
                ModuleRoute[moduleType] = moduleType.Name.Replace("Module", "");
            }
            else
            {
                ModuleRoute[moduleType] = routeAttribute.Template;
            }
        }

        /// <summary>
        ///     Gets the main route for a module
        /// </summary>
        /// <returns>The main route</returns>
        private string GetRoute()
        {
            var type = this.GetType();
            return ModuleRoute.ContainsKey(type) ? ModuleRoute[type] : string.Empty;
        }

        /// <summary>
        ///     Get a <see cref="Guid" /> based on a name from the <see cref="HttpRequest" />
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" /></param>
        /// <param name="routeKey">The key where the <see cref="Guid" /> is stored</param>
        /// <returns>The <see cref="Guid" /></returns>
        protected Guid GetAdditionalRouteId(HttpRequest request, string routeKey)
        {
            return new Guid((string)request.RouteValues[routeKey] ?? string.Empty);
        }
    }
}
