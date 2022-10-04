// --------------------------------------------------------------------------------------------------------
// <copyright file="FileService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.FileService
{
    using CDP4Dal;
    using CDP4Dal.DAL;

    using CDP4JsonFileDal;

    /// <summary>
    ///     This service provides capabilities to write and read <see cref="File" /> that represents Annex C files or Budget Templates
    /// </summary>
    public class FileService : IFileService
    {
        /// <summary>
        ///     The name of the temporary folder
        /// </summary>
        private const string TempFolderName = "tmp";

        /// <summary>
        ///     The main path to store files
        /// </summary>
        private readonly string mainPath;

        /// <summary>
        ///     Initializes a new <see cref="FileService" />
        /// </summary>
        /// <param name="storagePath">The path where the <see cref="FileService" /> should store files</param>
        public FileService(string storagePath)
        {
            if (storagePath == null)
            {
                throw new ArgumentNullException(nameof(storagePath));
            }

            if (!Path.IsPathFullyQualified(storagePath))
            {
                storagePath = Path.GetFullPath(storagePath);
            }

            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            this.mainPath = storagePath;
        }

        /// <summary>
        ///     Verifies if the <see cref="File" /> at the provided <paramref name="fileName" /> correspond to a valid Annex C file
        /// </summary>
        /// <param name="fileName">The path of the file</param>
        /// <returns>A <see cref="Task" /> with the result of the verification</returns>
        public async Task<bool> IsAnnexC3File(string fileName)
        {
            try
            {
                var credentials = new Credentials("admin", "pass", new Uri(Path.Combine(this.GetTempFolder(), fileName)));
                var dal = new JsonFileDal();
                var session = new Session(dal, credentials);
                await session.Open();
                var isSessionOpen = session.RetrieveSiteDirectory() != null;
                await session.Close();
                return isSessionOpen;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Deletes the <see cref="File" /> at the provided <paramref name="filePath" /> if it exists
        /// </summary>
        /// <param name="filePath">The path of the <see cref="File" /></param>
        public void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(this.mainPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        ///     Deletes a temporary <see cref="File" />
        /// </summary>
        /// <param name="fileName">The name of the <see cref="File" /></param>
        public void DeleteTemporaryFile(string fileName)
        {
            var fullPath = Path.Combine(this.GetTempFolder(), fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        /// <summary>
        ///     Verifies if a <see cref="File" /> already exists
        /// </summary>
        /// <param name="folderPath">The path of the folder</param>
        /// <param name="fileName">The <see cref="File" /> name</param>
        /// <returns>A value indicating if the <see cref="File" /> exists</returns>
        public bool Exists(string folderPath, string fileName)
        {
            return File.Exists(Path.Combine(this.mainPath, folderPath, fileName));
        }

        /// <summary>
        ///     Gets the path of the Temporary folder
        /// </summary>
        /// <returns>The path of the folder</returns>
        public string GetTempFolder()
        {
            var path = Path.Combine(this.mainPath, TempFolderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        ///     Verifies if a temporary <see cref="File" /> exists
        /// </summary>
        /// <param name="tempFileName">The name of the temporary <see cref="File" /></param>
        /// <returns>True if exists</returns>
        public bool TempFileExists(string tempFileName)
        {
            return File.Exists(Path.Combine(this.GetTempFolder(), tempFileName));
        }

        /// <summary>
        ///     Moves a temporary <see cref="File" /> to its final destination
        /// </summary>
        /// <param name="tempFileName">The name of the temporary <see cref="File" /></param>
        /// <param name="newFolderPath">The folder destination</param>
        /// <returns>The new <see cref="File" /> location</returns>
        public string MoveFile(string tempFileName, string newFolderPath)
        {
            var destinationDirectory = Path.Combine(this.mainPath, newFolderPath);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            File.Move(Path.Combine(this.GetTempFolder(), tempFileName), Path.Combine(this.mainPath, newFolderPath, tempFileName));

            return Path.Combine(newFolderPath, tempFileName);
        }

        /// <summary>
        ///     Write the content of the <see cref="IFormFile" /> into a temporary file
        /// </summary>
        /// <param name="file">The <see cref="IFormFile" /></param>
        /// <param name="fileName">The name of the file</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task WriteToFile(IFormFile file, string fileName)
        {
            await using var fileStream = File.Create(Path.Combine(this.GetTempFolder(), fileName));
            await file.CopyToAsync(fileStream);
        }
    }
}
