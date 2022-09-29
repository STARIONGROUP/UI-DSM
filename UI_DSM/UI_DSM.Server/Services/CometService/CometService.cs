// --------------------------------------------------------------------------------------------------------
// <copyright file="CometService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.CometService
{
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using CDP4Dal;
    using CDP4Dal.DAL;
    using CDP4Dal.Exceptions;
    using CDP4Dal.Operations;

    using CDP4JsonFileDal;

    using CDP4ServicesDal;

    using UI_DSM.Client.Enumerator;
    using UI_DSM.Server.Services.FileService;
    using UI_DSM.Shared.DTO.CometData;

    /// <summary>
    ///     Service that provides helper methods related to Comet
    /// </summary>
    public class CometService : ICometService
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> to keep track of current <see cref="ISession" />
        /// </summary>
        private readonly Dictionary<Guid, ISession> currentSessions = new();

        /// <summary>
        ///     The <see cref="IFileService" />
        /// </summary>
        private readonly IFileService fileService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CometService" /> class.
        /// </summary>
        /// <param name="fileService">The <see cref="IFileService" /></param>
        public CometService(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        ///     Tries to login to a Comet <see cref="ISession" />
        /// </summary>
        /// <param name="authenticationData">The <see cref="CometAuthenticationData" /></param>
        /// <returns>A <see cref="Task" /> with the result of the login process</returns>
        public async Task<Tuple<AuthenticationStatus, Guid>> Login(CometAuthenticationData authenticationData)
        {
            var sessionId = Guid.NewGuid();
            var uri = new Uri(authenticationData.Url);
            var dal = new CdpServicesDal();
            var credentials = new Credentials(authenticationData.UserName, authenticationData.Password, uri);
            var session = new Session(dal, credentials);

            try
            {
                await session.Open();

                if (session.RetrieveSiteDirectory() == null)
                {
                    return Tuple.Create(AuthenticationStatus.Fail, Guid.Empty);
                }

                this.currentSessions[sessionId] = session;
                return Tuple.Create(AuthenticationStatus.Success, sessionId);
            }
            catch (DalReadException)
            {
                return Tuple.Create(AuthenticationStatus.Fail, Guid.Empty);
            }
            catch (HttpRequestException)
            {
                return Tuple.Create(AuthenticationStatus.ServerFailure, Guid.Empty);
            }
        }

        /// <summary>
        ///     Closes a <see cref="ISession" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /> to close</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task Close(Guid sessionId)
        {
            var session = this.GetSession(sessionId);

            foreach (var iteration in session.OpenIterations.Select(x => x.Key))
            {
                await session.CloseIterationSetup(iteration.IterationSetup);
            }

            await session.Close();
            this.currentSessions.Remove(sessionId);
        }

        /// <summary>
        ///     Gets a collection of available <see cref="EngineeringModelSetup" />
        /// </summary>
        /// <param name="sessionId">
        ///     The <see cref="Guid" /> of the <see cref="ISession" /> to get
        ///     <see cref="EngineeringModelSetup" />
        /// </param>
        /// <returns>A collection of <see cref="EngineeringModelSetup" /></returns>
        public IEnumerable<EngineeringModelSetup> GetAvailableEngineeringModel(Guid sessionId)
        {
            var session = this.GetSession(sessionId);

            var siteDirectory = session.RetrieveSiteDirectory();

            return siteDirectory.Model
                .Where(x => x.Participant.Any(p => p.IsActive && p.Person.Iid == session.ActivePerson.Iid)
                            && x.IterationSetup.Any(iteration => !iteration.IsDeleted && iteration.FrozenOn != null));
        }

        /// <summary>
        ///     Reads an <see cref="IterationSetup" /> to get the corresponding
        ///     <see cref="Iteration" />
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /></param>
        /// <param name="iterationSetup">The <see cref="IterationSetup" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Iteration" /></returns>
        public async Task<Iteration> ReadIteration(Guid sessionId, IterationSetup iterationSetup)
        {
            var session = this.GetSession(sessionId);

            var modelSetup = (EngineeringModelSetup)iterationSetup.Container;
            var participant = modelSetup.Participant.FirstOrDefault(x => x.IsActive && x.Person.Iid == session.ActivePerson.Iid);

            if (participant == null)
            {
                throw new InvalidOperationException("The logged in Person is not part of the provided IterationSetup");
            }

            var model = new EngineeringModel(modelSetup.EngineeringModelIid, session.Assembler.Cache, session.Credentials.Uri);
            var iteration = new Iteration(iterationSetup.IterationIid, session.Assembler.Cache, session.Credentials.Uri);
            model.Iteration.Add(iteration);

            await session.Read(iteration, participant.SelectedDomain);

            if (session.OpenIterations.All(x => x.Key.Iid != iteration.Iid))
            {
                throw new InvalidOperationException("An error occured during opening the iteration");
            }

            return session.OpenIterations.First(x => x.Key.Iid == iteration.Iid).Key;
        }

        /// <summary>
        ///     Creates an Annex C3 file for the provided iteration
        /// </summary>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <returns>A <see cref="Task" /> with the name of the created file</returns>
        public async Task<string> CreateAnnexC3File(Iteration iteration)
        {
            var fileName = $"{iteration.Iid}.zip";
            var path = Path.Combine(this.fileService.GetTempFolder(),fileName);
            var credentials = new Credentials("admin", "pass", new Uri(path));
            var dal = new JsonFileDal();
            _ = new Session(dal, credentials);

            var operationContainers = new List<OperationContainer>();
            var transactionContext = TransactionContextResolver.ResolveContext(iteration);
            var operationContainer = new OperationContainer(transactionContext.ContextRoute());
            var dto = iteration.ToDto();

            var operation = new Operation(null, dto, OperationKind.Create);
            operationContainer.AddOperation(operation);
            operationContainers.Add(operationContainer);
            await dal.Write(operationContainers);
            return fileName;
        }

        /// <summary>
        ///     Gets an <see cref="IterationSetup" /> based on provided <see cref="Guid" />s
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /></param>
        /// <param name="modelId">The <see cref="Guid" /> of the <see cref="EngineeringModelSetup" /></param>
        /// <param name="iterationId">The <see cref="Guid" /> of the <see cref="IterationSetup" /></param>
        /// <returns>The <see cref="IterationSetup" /></returns>
        public IterationSetup GetIterationSetup(Guid sessionId, Guid modelId, Guid iterationId)
        {
            var session = this.GetSession(sessionId);
            var siteDirectory = session.RetrieveSiteDirectory();
            var model = siteDirectory.Model.FirstOrDefault(x => x.Iid == modelId);

            if (model == null)
            {
                throw new ArgumentException("Invalid model id");
            }

            var iteration = model.IterationSetup.FirstOrDefault(x => x.IterationIid == iterationId);

            if (iteration == null)
            {
                throw new ArgumentException("Invalid iteration id");
            }

            return iteration;
        }

        /// <summary>
        ///     Gets the correct <see cref="ISession" />.
        /// </summary>
        /// <param name="sessionId">The <see cref="Guid" /> of the <see cref="ISession" /></param>
        /// <returns>The <see cref="ISession" /></returns>
        /// <exception cref="ArgumentException">
        ///     If the provided <see cref="Guid" /> does not correspond to any
        ///     <see cref="ISession" />
        /// </exception>
        private ISession GetSession(Guid sessionId)
        {
            if (!this.currentSessions.TryGetValue(sessionId, out var session))
            {
                throw new ArgumentException("Invalid session id");
            }

            return session;
        }
    }
}
