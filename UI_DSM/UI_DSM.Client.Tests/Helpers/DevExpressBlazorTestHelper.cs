// --------------------------------------------------------------------------------------------------------
// <copyright file="DevExpressBlazorTestHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Helpers
{
    using System.Diagnostics.CodeAnalysis;

    using Bunit;

    using DevExpress.Blazor.Internal;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Helper class that configures a <see cref="TestContext" /> to be able to test DevExpress components
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DevExpressBlazorTestHelper
    {
        /// <summary>
        ///     Configure the <see cref="TestContext" /> to include all prerequisites to test components with DevExpress components
        /// </summary>
        /// <param name="context">The <see cref="TestContext" /></param>
        public static void ConfigureDevExpressBlazor(this TestContext context)
        {
            context.Services.AddDevExpressBlazor(_ => ConfigureJSInterop(context.JSInterop));
        }

        /// <summary>
        ///     Restore the <see cref="TestContext.JSInterop" /> and disposes the <see cref="TestContext" />
        /// </summary>
        /// <param name="context">The <see cref="TestContext" /></param>
        public static void CleanContext(this TestContext context)
        {
            context.JSInterop.Mode = JSRuntimeMode.Strict;
            context.Dispose();
        }

        /// <summary>
        ///     Configure the <see cref="BunitJSInterop" /> for DevExpress
        /// </summary>
        /// <param name="interop">The <see cref="BunitJSInterop" /> to configure</param>
        private static void ConfigureJSInterop(BunitJSInterop interop)
        {
            interop.Mode = JSRuntimeMode.Loose;
            var rootModule = interop.SetupModule("./_content/DevExpress.Blazor/dx-blazor.js");
            rootModule.Mode = JSRuntimeMode.Strict;
            
            rootModule.Setup<DeviceInfo>("getDeviceInfo", _ => true)
                .SetResult(new DeviceInfo(false));

            rootModule.SetupModule("DxBlazor.Modal.getReference", _ => true);
        }
    }
}
