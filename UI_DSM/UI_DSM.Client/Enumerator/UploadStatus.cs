// --------------------------------------------------------------------------------------------------------
// <copyright file="UploadStatus.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Enumerator
{
    /// <summary>
    ///     Enumerator for the current status of an upload process
    /// </summary>
    public enum UploadStatus
    {
        /// <summary>
        ///     When the upload is not started
        /// </summary>
        None,

        /// <summary>
        ///     When the upload has been started
        /// </summary>
        Uploading,

        /// <summary>
        ///     When the upload is finished
        /// </summary>
        Done,

        /// <summary>
        ///     When the upload has failed
        /// </summary>
        Fail
    }
}
