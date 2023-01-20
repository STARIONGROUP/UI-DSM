// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingExtensionTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using NUnit.Framework;

    using UI_DSM.Client.Extensions;

    [TestFixture]
    public class ThingExtensionTestFixture
    {
        [Test]
        public void VerifyGetSimpleParameterValue()
        {
            var requirement = new Requirement()
            {
                ParameterValue =
                {
                    new SimpleParameterValue()
                    {
                        ParameterType = new TextParameterType()
                        {
                            Name = "Justification"
                        },
                        Value = new ValueArray<string>(new List<string>{"A justificiation"})
                    },
                    new SimpleParameterValue()
                    {
                        ParameterType = new TextParameterType()
                        {
                            Name = "type"
                        },
                    }
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetSimpleParameterValue("justification"), Is.Not.Empty);
                Assert.That(requirement.GetSimpleParameterValue("type"), Is.Null);
                Assert.That(requirement.GetSimpleParameterValue("something"), Is.Null);
            });
        }

        [Test]
        public void VerifyGetCategories()
        {
            var categories = new List<Category>
            {
                new ()
                {
                    Iid = Guid.NewGuid(),
                    IsDeprecated = true,
                    Name = "DeprecatedCategory"
                },
                new ()
                {
                    Iid = Guid.NewGuid(),
                    Name = "UndeprecatedCategory"
                }
            };

            var requirement = new Requirement()
            {
                Category = categories
            };

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetCategories().ToList(), Has.Count.EqualTo(1));
                Assert.That(requirement.GetCategories(true).ToList(), Has.Count.EqualTo(2));
            });
        }

        [Test]
        public void VerifyGetParametricConstraints()
        {
            var requirement = new Requirement()
            {
                ParametricConstraint =
                {
                    new ParametricConstraint()
                    {
                        Expression =
                        {
                            new AndExpression(),
                            new AndExpression(),
                        }
                    },
                    new ParametricConstraint()
                    {
                        Expression =
                        {
                            new OrExpression()
                        }
                    }
                }
            };

            Assert.That(requirement.GetParametricConstraints().ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public void VerifyGetOwner()
        {
            var requirement = new Requirement()
            {
                Owner = new DomainOfExpertise()
                {
                    ShortName = "SYS"
                }
            };

            Assert.That(requirement.GetOwnerShortName(), Is.EqualTo("SYS"));
        }

        [Test]
        public void VerifyGetSingleDefinition()
        {
            var requirement = new Requirement();
            const string englishContent = "A content";
            const string frenchContent = "Le contenu d'une définition";
            Assert.That(requirement.GetFirstDefinition(), Is.Null);

            requirement.Definition.Add(new Definition()
            {
                LanguageCode = "en",
                Content = englishContent
            });

            requirement.Definition.Add(new Definition()
            {
                LanguageCode = "fr",
                Content = frenchContent
            });

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetFirstDefinition(), Is.EqualTo(englishContent));
                Assert.That(requirement.GetFirstDefinition("fr"), Is.EqualTo(frenchContent));
                Assert.That(requirement.GetFirstDefinition("nl"), Is.EqualTo(englishContent));
            });
        }

        [Test]
        public void VerifyGetAvailableViews()
        {
            var views = new Requirement().GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(5));
            var elementUsage = new ElementUsage();
            views = elementUsage.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(2));

            elementUsage.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.ProductCategoryName
                }
            };

            views = elementUsage.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(5));

            elementUsage.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.FunctionCategoryName
                }
            };

            views = elementUsage.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(3));

            elementUsage.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.PortCategoryName
                }
            };

            views = elementUsage.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));

            views = new HyperLink().GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));

            var relationship = new BinaryRelationship()
            {
                Category = new List<Category>
                {
                    new()
                    {
                        Name = ThingExtension.InterfaceCategoryName
                    }
                }, 
                Source = new ElementUsage()
                {
                    Category = new List<Category>
                    {
                        new()
                        {
                            Name = ThingExtension.PortCategoryName
                        }
                    }
                },
                Target = new ElementUsage()
                {
                    Category = new List<Category>
                    {
                        new()
                        {
                            Name = ThingExtension.PortCategoryName
                        }
                    }
                }
            };

            views = relationship.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));

            relationship.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.DeriveCategoryName
                }
            };

            views = relationship.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(2));

            relationship.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.TraceCategoryName
                }
            };

            views = relationship.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));

            relationship.Category = new List<Category>
            {
                new()
                {
                    Name = ThingExtension.SatisfyCategoryName
                }
            };

            relationship.Source = new ElementDefinition()
            {
                Category = new List<Category>
                {
                    new()
                    {
                        Name = ThingExtension.ProductCategoryName
                    }
                }
            };

            views = relationship.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));

            relationship.Source = new ElementDefinition()
            {
                Category = new List<Category>
                {
                    new()
                    {
                        Name = ThingExtension.FunctionCategoryName
                    }
                }
            };

            views = relationship.GetAvailableViews();
            Assert.That(views.ToList(), Has.Count.EqualTo(1));
        }
    }
}
