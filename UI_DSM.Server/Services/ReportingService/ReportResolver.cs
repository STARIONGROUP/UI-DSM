// --------------------------------------------------------------------------------------------------------
// <copyright file="ReportResolver.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Services.ReportingService
{
    using CDP4Common.EngineeringModelData;

    using CDP4Reporting.DynamicTableChecker;
    using CDP4Reporting.ReportScript;

    using DevExpress.XtraReports.UI;

    using UI_DSM.Server.Reporting.CodeCompiler;
    using UI_DSM.Server.Reporting.DynamicTableChecker;
    using UI_DSM.Server.Reporting.ReportScriptHandler;

    using Parameter = DevExpress.XtraReports.Parameters.Parameter;

    /// <summary>
    /// Resolves a report that is stored somewhere and returns
    /// </summary>
    public class ReportResolver
    {
        /// <summary>
        /// The <see cref="IDynamicTableChecker{T}"/> of type <see cref="XtraReport"/>
        /// </summary>
        private IDynamicTableChecker<XtraReport> dynamicTableChecker = new DynamicTableChecker();

        /// <summary>
        /// Resolve the <see cref="XtraReport"/>
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="session">The <see cref="ISession"/></param>
        /// <param name="compileDataSource">Indication that the datasource needs to be compiled</param>
        /// <returns>The generated <see cref="XtraReport"/></returns>
        public XtraReport Resolve(string fileName, Iteration iteration, CDP4Dal.ISession session, bool compileDataSource = true)
        {
            this.Report = null;
            this.Errors = string.Empty;
            this.Output = string.Empty;

            if (fileName == null)
            {
                return this.Report;
            }

            this.Report = new XtraReport();

            this.ReportScriptHandler = 
                new ReportScriptHandler<XtraReport, Parameter>
                (
                    new XtraReportHandler(this.Report), 
                    new RoslynCodeCompiler(x => { this.Output += $"{x}\n"; }),
                    x => { this.Errors = x; }, 
                    x => { this.Output += $"{x}\n"; });

            using (var reportZipArchive = this.ReportScriptHandler.GetReportZipArchive(fileName))
            {
                using (var binaryReader = new BinaryReader(reportZipArchive.ReportDefinition))
                {
                    this.Report.LoadLayoutFromXml(reportZipArchive.ReportDefinition);
                }

                if (compileDataSource && reportZipArchive.DataSourceCode != null)
                {
                    using (var streamReader = new StreamReader(reportZipArchive.DataSourceCode))
                    {
                        var datasourceCode = streamReader.ReadToEnd();

                        this.ReportScriptHandler.CompileAssembly(datasourceCode);

                        this.ReportScriptHandler.RebuildDataSource(iteration, session);

                        this.CheckDynamicTables();
                    }
                }

                return this.Report;
            }
        }

        /// <summary>
        /// Checks the report if dynamic tables need to be updated.
        /// </summary>
        private void CheckDynamicTables()
        {
            this.dynamicTableChecker.Check(this.Report, this.ReportScriptHandler.CurrentDataCollector);
        }

        /// <summary>
        /// Gets or sets the errors
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        /// Gets or sets the output
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ReportScriptHandler"/>
        /// </summary>
        private ReportScriptHandler<XtraReport, Parameter> ReportScriptHandler { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="XtraReport"/>
        /// </summary>
        private XtraReport Report { get; set; }
    }
}
