// --------------------------------------------------------------------------------------------------------
// <copyright file="MockIJSRuntime.cs" company="RHEA System S.A.">
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
    using System.Globalization;

    using DevExpress.Blazor.Internal;

    using Microsoft.AspNetCore.Components.Rendering;
    using Microsoft.JSInterop;

    public class MockIJSRuntime : IJSRuntime, IEnvironmentInfoFactory
    {
        public MockIJSRuntime()
        {
            this.EnvironmentInfo = new MockEnvironmentInfo();
        }

        public IEnvironmentInfo EnvironmentInfo { get; }

        public List<string> InvokeHistory { get; } = new();

        public IEnvironmentInfo CreateEnvironmentInfo()
        {
            return this.EnvironmentInfo;
        }

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object[] args)
        {
            this.InvokeHistory.Add(identifier);

            if (typeof(TValue) == typeof(ApiScheme))
            {
                return (TValue)(object)await this.EnvironmentInfo.ApiScheme;
            }

            if (typeof(TValue) == typeof(DeviceInfo))
            {
                return (TValue)(object)await this.EnvironmentInfo.DeviceInfo;
            }

            return default;
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
            object[] args)
        {
            this.InvokeHistory.Add(identifier);
            return new ValueTask<TValue>(default(TValue));
        }
    }

    public class MockEnvironmentInfo : IEnvironmentInfo
    {
        public static readonly DateTime DateTimeNow = DateTime.Now.Date;

        public DateTime GetDateTimeNow()
        {
            return DateTimeNow;
        }

        public CultureInfo CurrentCulture { get; }

        public Task<ApiScheme> ApiScheme
        {
            get { return new Task<ApiScheme>(() => new ApiScheme(true)); }
        }

        public Task<DeviceInfo> DeviceInfo
        {
            get { return new Task<DeviceInfo>(() => new DeviceInfo(false)); }
        }

        public Task InitializeRuntime()
        {
            return Task.CompletedTask;
        }

        public void RenderScriptLoader(RenderTreeBuilder builder, Guid containerId)
        {
        }

        public string ResolveUrl(string url)
        {
            return url;
        }

        public void OnMessage(int msg)
        {
        }

        public void Dispose()
        {
        }
    }
}
