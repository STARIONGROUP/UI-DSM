// --------------------------------------------------------------------------------------------------------
// <copyright file="CometSessionService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.CometSessionService
{
    using CDP4Dal;
    using CDP4Dal.DAL;

    using CDP4ServicesDal;

    using ReactiveUI;

    /// <summary>
    /// The <see cref="CometSessionService" /> provides access to a Comet <see cref="ISession"/> 
    /// </summary>
    public class CometSessionService : ReactiveObject, ICometSessionService
    {
        /// <summary>
        /// Backing field for <see cref="IsSessionOpen"/>
        /// </summary>
        private bool isSessionOpen;

        /// <summary>
        /// Gets or sets the <see cref="ISession"/>
        /// </summary>
        public ISession? Session { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if an <see cref="ISession"/> is opened
        /// </summary>
        public bool IsSessionOpen
        {
            get => this.isSessionOpen;
            set => this.RaiseAndSetIfChanged(ref this.isSessionOpen, value);
        }

        /// <summary>
        /// Opens an <see cref="ISession"/>
        /// </summary>
        /// <param name="credentials">The <see cref="Credentials" /> to open the <see cref="ISession"/></param>
        /// <returns>A <see cref="Task"/> with the value asserting if the <see cref="ISession"/> has been successfully opened</returns>
        public async Task<bool> Open(Credentials credentials)
        {
            this.Session = new Session(new CdpServicesDal(), credentials);

            await this.Session.Open();

            this.IsSessionOpen = this.Session?.RetrieveSiteDirectory() != null;
            return this.IsSessionOpen;
        }

        /// <summary>
        /// Close the <see cref="ISession"/>
        /// </summary>
        /// <returns>A <see cref="Task"/></returns>
        public async Task Close()
        {
            if (this.Session != null)
            {
                await this.Session.Close();
            }

            this.IsSessionOpen = false;
        }
    }
}
