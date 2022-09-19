// --------------------------------------------------------------------------------------------------------
// <copyright file="DtoGeneratorTestFixture.cs" company="RHEA System S.A.">
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
    public class DtoGeneratorTestFixture
    {
        private DirectoryInfo dtoDirectoryInfo;
        private DtoGenerator dtoGenerator;

        [SetUp]
        public void SetUp()
        {
            var outputpath = TestContext.CurrentContext.TestDirectory;
            var directoryInfo = new DirectoryInfo(outputpath);
            this.dtoDirectoryInfo = directoryInfo.CreateSubdirectory("AutoGenDto");

            this.dtoGenerator = new DtoGenerator();
        }

        [Test]
        public void VerifyClassesGeneration()
        {
            Assert.That(async () => await this.dtoGenerator.Generate(this.dtoDirectoryInfo),
                Throws.Nothing);
        }
    }
}
