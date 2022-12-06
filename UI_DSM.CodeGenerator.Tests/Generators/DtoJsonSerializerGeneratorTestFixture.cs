// --------------------------------------------------------------------------------------------------------
// <copyright file="DtoJsonSerializerGeneratorTestFixture.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.CodeGenerator.Tests.Generators
{
    using NUnit.Framework;

    using UI_DSM.CodeGenerator.Generators;

    [TestFixture]
    public class DtoJsonSerializerGeneratorTestFixture
    {
        private DirectoryInfo dtoDirectoryInfo;
        private DtoJsonSerializerGenerator dtoSerializerGenerator;

        [SetUp]
        public void SetUp()
        {
            var outputpath = TestContext.CurrentContext.TestDirectory;
            var directoryInfo = new DirectoryInfo(outputpath);
            this.dtoDirectoryInfo = directoryInfo.CreateSubdirectory("AutoGenDtoSerializer");

            this.dtoSerializerGenerator = new DtoJsonSerializerGenerator();
        }

        [Test]
        public void VerifyClassesGeneration()
        {
            Assert.That(async () => await this.dtoSerializerGenerator.Generate(this.dtoDirectoryInfo),
                Throws.Nothing);
        }
    }
}
