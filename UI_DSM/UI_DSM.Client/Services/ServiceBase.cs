// --------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services
{
    using System.Text.Json;

    using Microsoft.AspNetCore.Components;

    /// <summary>
    ///     Base class for any service that needs to access to the API
    /// </summary>
    public abstract class ServiceBase
    {
        /// <summary>
        ///     The <see cref="HttpClient" /> to access to the server API
        /// </summary>
        protected HttpClient HttpClient;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" /> that is used for JSON action
        /// </summary>
        protected JsonSerializerOptions JsonSerializerOptions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceBase" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        protected ServiceBase(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.JsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            this.MainRoute = this.GetRoute();
        }

        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> that store the main route for all <see cref="ServiceBase" />
        /// </summary>
        private static Dictionary<Type, string> ServiceRoute { get; } = new();

        /// <summary>
        ///     Main Route for the <see cref="ServiceBase" />
        /// </summary>
        protected string MainRoute { get; private set; }

        /// <summary>
        ///     Register a <see cref="Type" /> if this <see cref="Type" /> derives from <see cref="ServiceBase" />
        /// </summary>
        /// <param name="serviceType">The <see cref="Type" /> to register</param>
        public static void RegisterService(Type serviceType)
        {
            if (!serviceType.IsSubclassOf(typeof(ServiceBase)))
            {
                return;
            }

            var routeAttribute = (RouteAttribute)Attribute.GetCustomAttribute(serviceType, typeof(RouteAttribute));

            if (routeAttribute == null)
            {
                ServiceRoute[serviceType] = serviceType.Name.Replace("Service", "");
            }
            else
            {
                ServiceRoute[serviceType] = routeAttribute.Template;
            }
        }

        /// <summary>
        ///     Register a <see cref="Type" /> if this <see cref="Type" /> derives from <see cref="ServiceBase" />
        /// </summary>
        public static void RegisterService<T>()
        {
            RegisterService(typeof(T));
        }

        /// <summary>
        ///     Gets the main route for a service
        /// </summary>
        /// <returns>The main route</returns>
        private string GetRoute()
        {
            var type = this.GetType();
            return ServiceRoute.ContainsKey(type) ? ServiceRoute[type] : string.Empty;
        }
    }
}
