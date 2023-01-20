// --------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValuesVisualizerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.App.ParameterValuesVisualizer
{
    using Bunit;

    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;
    
    using NUnit.Framework;

    using UI_DSM.Client.Components.App.ParameterValuesVisualizer;
    using UI_DSM.Client.Tests.Helpers;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ParameterValuesVisualizerTestFixture
    {
        private TestContext context;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyComponentWithScalarParameterType()
        {
            var scalar = new BooleanParameterType()
            {
                Name = "bool param",
                ShortName = "bp"
            };

            var parameter = new Parameter()
            {
                ParameterType = scalar
            };
            
            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string>{"true"}),
                ValueSwitch = ParameterSwitchKind.MANUAL
            });

            var renderer = this.context.RenderComponent<ParameterValuesVisualizer>(parameters =>
            {
                parameters.Add(p => p.Parameter, parameter);
                parameters.Add(p => p.Option, new Option());
            });

            Assert.That(renderer.FindAll("button"), Is.Empty);

            parameter.StateDependence = new ActualFiniteStateList()
            {
                ActualState =
                {
                    new ActualFiniteState(),
                    new ActualFiniteState()
                }
            };

            parameter.ValueSet.Clear();

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "true" }),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                ActualState = parameter.StateDependence.ActualState.First()
            });

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "false" }),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                ActualState = parameter.StateDependence.ActualState.Last()
            });

            renderer = this.context.RenderComponent<ParameterValuesVisualizer>(parameters =>
            {
                parameters.Add(p => p.Parameter, parameter);
                parameters.Add(p => p.Option, new Option());
            });

            Assert.That(() => renderer.Find("button"), Throws.Nothing);
        }

        [Test]
        public void VerifyComponentWithQuantityKindParameterType()
        {
            var scale = new RatioScale()
            {
                Name = "kilogram",
                ShortName = "kg"
            };

            var scalar = new SimpleQuantityKind()
            {
                Name = "bool param",
                ShortName = "bp",
                PossibleScale = {scale}
            };

            var parameter = new Parameter()
            {
                ParameterType = scalar,
                Scale = scale
            };

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "4.5" }),
                ValueSwitch = ParameterSwitchKind.MANUAL
            });

            var renderer = this.context.RenderComponent<ParameterValuesVisualizer>(parameters =>
            {
                parameters.Add(p => p.Parameter, parameter);
                parameters.Add(p => p.Option, new Option());
            });

            Assert.That(renderer.FindAll("button"), Is.Empty);

            parameter.StateDependence = new ActualFiniteStateList()
            {
                ActualState =
                {
                    new ActualFiniteState(),
                    new ActualFiniteState()
                }
            };

            parameter.ValueSet.Clear();

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "4.5" }),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                ActualState = parameter.StateDependence.ActualState.First()
            });

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "4.8" }),
                ValueSwitch = ParameterSwitchKind.MANUAL,
                ActualState = parameter.StateDependence.ActualState.Last()
            });

            renderer = this.context.RenderComponent<ParameterValuesVisualizer>(parameters =>
            {
                parameters.Add(p => p.Parameter, parameter);
                parameters.Add(p => p.Option, new Option());
            });

            Assert.That(() => renderer.Find("button"), Throws.Nothing);
        }

        [Test]
        public void VerifyComponentWithCompoundParameterType()
        {
            var compound = new CompoundParameterType()
            {
                Name = "Dimension",
            };

            compound.Component.Add(new ParameterTypeComponent()
            {
                ShortName = "DimX"
            });

            compound.Component.Add(new ParameterTypeComponent()
            {
                ShortName = "DimY"
            });

            compound.Component.Add(new ParameterTypeComponent()
            {
                ShortName = "DimZ"
            });

            var parameter = new Parameter()
            {
                ParameterType = compound
            };

            parameter.ValueSet.Add(new ParameterValueSet()
            {
                Manual = new ValueArray<string>(new List<string> { "4.5", "4.2", "4.3" }),
                ValueSwitch = ParameterSwitchKind.MANUAL
            });

            var renderer = this.context.RenderComponent<ParameterValuesVisualizer>(parameters =>
            {
                parameters.Add(p => p.Parameter, parameter);
                parameters.Add(p => p.Option, new Option());
            });

            Assert.That(() => renderer.Find("button"), Throws.Nothing);
        }
    }
}
