// --------------------------------------------------------------------------------------------------------
// <copyright file="IFileService.cs" company="RHEA System S.A.">
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
    /// <summary>
    ///     Interface definition for <see cref="FileService" />
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        ///     Verifies if the <see cref="File" /> at the provided <paramref name="fileName" /> correspond to a valid Annex C file
        /// </summary>
        /// <param name="fileName">The path of the file</param>
        /// <returns>A <see cref="Task" /> with the result of the verification</returns>
        Task<bool> IsAnnexC3File(string fileName);

        /// <summary>
        ///     Deletes the <see cref="File" /> at the provided <paramref name="filePath" /> if it exists
        /// </summary>
        /// <param name="filePath">The path of the <see cref="File" /></param>
        void DeleteFile(string filePath);

        /// <summary>
        ///     Gets the path of the Temporary folder
        /// </summary>
        /// <returns>The path of the folder</returns>
        string GetTempFolder();

        /// <summary>
        ///     Verifies if a temporary <see cref="File" /> exists
        /// </summary>
        /// <param name="tempFileName">The name of the temporary <see cref="File" /></param>
        /// <returns>True if exists</returns>
        bool TempFileExists(string tempFileName);

        /// <summary>
        ///     Moves a temporary <see cref="File" /> to its final destination
        /// </summary>
        /// <param name="tempFileName">The name of the temporary <see cref="File" /></param>
        /// <param name="newFolderPath">The folder destination</param>
        /// <returns>The new <see cref="File" /> location</returns>
        string MoveFile(string tempFileName, string newFolderPath);

        /// <summary>
        ///     Deletes a temporary <see cref="File" />
        /// </summary>
        /// <param name="fileName">The name of the <see cref="File" /></param>
        void DeleteTemporaryFile(string fileName);

        /// <summary>
        ///     Verifies if a <see cref="File" /> already exists
        /// </summary>
        /// <param name="folderPath">The path of the folder</param>
        /// <param name="fileName">The <see cref="File" /> name</param>
        /// <returns>A value indicating if the <see cref="File" /> exists</returns>
        bool Exists(string folderPath, string fileName);

        /// <summary>
        ///     Write the content of the <see cref="IFormFile" /> into a temporary file
        /// </summary>
        /// <param name="file">The <see cref="IFormFile" /></param>
        /// <param name="fileName">The name of the file</param>
        /// <returns>A <see cref="Task" /></returns>
        Task WriteToFile(IFormFile file, string fileName);

        /// <summary>
        ///     Retrieves the full path of the provides file
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <returns>The full path</returns>
        string GetFullPath(string fileName);

        /// <summary>
        ///     Retrieves the full path of a Directory
        /// </summary>
        /// <param name="directoryName">The directory name</param>
        /// <returns>The full path</returns>
        string GetDirectoryPath(string directoryName);
    }
}
