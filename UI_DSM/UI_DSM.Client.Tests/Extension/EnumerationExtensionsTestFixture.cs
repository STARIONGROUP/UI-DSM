// --------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationExtensionsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Extension
{
    using NUnit.Framework;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Extensions;

    [TestFixture]
    public class EnumerationExtensionsTestFixture
    {
        [Test]
        public void VerifyThatInterfaceCategoryCanBeConvertedToColorString()
        {
            Assert.Multiple(() =>
            {
                Assert.That(InterfaceCategory.Other.ToColorString(), Is.EqualTo("gray"));
                Assert.That(InterfaceCategory.Power_Interfaces.ToColorString(), Is.EqualTo("red"));
                Assert.That(InterfaceCategory.Signal_Interfaces.ToColorString(), Is.EqualTo("#00B0F0"));
                Assert.That(InterfaceCategory.TM_TC_Interfaces.ToColorString(), Is.EqualTo("#C85D7E"));
                Assert.That(InterfaceCategory.DataBus_Interfaces.ToColorString(), Is.EqualTo("yellow"));
                Assert.That(InterfaceCategory.Str_Interfaces.ToColorString(), Is.EqualTo("black"));
                Assert.That(InterfaceCategory.TC_Interfaces.ToColorString(), Is.EqualTo("#843C0C"));
                Assert.That(InterfaceCategory.Mechanisms_Interfaces.ToColorString(), Is.EqualTo("#7030A0"));
                Assert.That(InterfaceCategory.Prop_Interfaces.ToColorString(), Is.EqualTo("#FFC000"));
                Assert.That(InterfaceCategory.Comms_Interfaces.ToColorString(), Is.EqualTo("#99FFCC"));
            });
        }

        [Test]
        public void VerifyThatInterfaceCategoryCanBeConvertedToName()
        {
            Assert.Multiple(() =>
            {
                Assert.That(InterfaceCategory.Other.ToName(), Is.EqualTo("Other"));
                Assert.That(InterfaceCategory.Power_Interfaces.ToName(), Is.EqualTo("Power"));
                Assert.That(InterfaceCategory.Signal_Interfaces.ToName(), Is.EqualTo("Signal"));
                Assert.That(InterfaceCategory.TM_TC_Interfaces.ToName(), Is.EqualTo("Tele-metry/command"));
                Assert.That(InterfaceCategory.DataBus_Interfaces.ToName(), Is.EqualTo("Data Bus"));
                Assert.That(InterfaceCategory.Str_Interfaces.ToName(), Is.EqualTo("Structural"));
                Assert.That(InterfaceCategory.TC_Interfaces.ToName(), Is.EqualTo("Thermal Control"));
                Assert.That(InterfaceCategory.Mechanisms_Interfaces.ToName(), Is.EqualTo("Mechanisms"));
                Assert.That(InterfaceCategory.Prop_Interfaces.ToName(), Is.EqualTo("Propulsion"));
                Assert.That(InterfaceCategory.Comms_Interfaces.ToName(), Is.EqualTo("Communications"));

            });
        }
    }
}
