// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableOfGuidRouteConstraintTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Extensions
{
    using Microsoft.AspNetCore.Routing;

    using NUnit.Framework;

    using UI_DSM.Server.Extensions;

    [TestFixture]
    public class EnumerableOfGuidRouteConstraintTestFixture
    {
        private readonly EnumerableOfGuidRouteConstraint constraint = new ();

        [Test]
        public void VerifyMatch()
        {
            const string guidsKey = "guids";
           
            Assert.Multiple(() =>
            {
                Assert.That(() => this.constraint.Match(null, null, null, null, RouteDirection.IncomingRequest), Throws.ArgumentNullException);
                Assert.That(() => this.constraint.Match(null, null, guidsKey, null, RouteDirection.IncomingRequest), Throws.ArgumentNullException);
            });

            var values = new RouteValueDictionary { { "guid", Guid.NewGuid().ToString() } };
            Assert.That(this.constraint.Match(null, null, guidsKey, values, RouteDirection.IncomingRequest), Is.False);
            var invalidStart = $"{Guid.NewGuid()}]";
            values[guidsKey] = invalidStart;
            Assert.That(this.constraint.Match(null, null, guidsKey, values, RouteDirection.IncomingRequest), Is.False);

            var invalidEnd = $"[{Guid.NewGuid()}";
            values[guidsKey] = invalidEnd;
            Assert.That(this.constraint.Match(null, null, guidsKey, values, RouteDirection.IncomingRequest), Is.False);

            var invalidContent = $"[{Guid.NewGuid()}{Guid.NewGuid()}]";
            values[guidsKey] = invalidContent;
            Assert.That(this.constraint.Match(null, null, guidsKey, values, RouteDirection.IncomingRequest), Is.False);

            var validContent = $"[{Guid.NewGuid()};{Guid.NewGuid()}]";
            values[guidsKey] = validContent;
            Assert.That(this.constraint.Match(null, null, guidsKey, values, RouteDirection.IncomingRequest), Is.True);
        }
    }
}
