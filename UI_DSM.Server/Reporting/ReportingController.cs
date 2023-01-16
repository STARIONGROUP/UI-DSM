// -------------------------------------------------------------------------------------------------------
// <copyright file="ReportingController.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Reporting
{
    using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
    using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

    // This controller is required for the Document Viewer and Report Designer.
    public class ReportingController : WebDocumentViewerController {
        public ReportingController(IWebDocumentViewerMvcControllerService controllerService) : base(controllerService)
        {
        }
    }
}
