// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.ProjectManager
{
    using Microsoft.EntityFrameworkCore;

    using NLog;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Project" />s
    /// </summary>
    public class ProjectManager : IProjectManager
    {
        /// <summary>
        ///     The <see cref="DatabaseContext" />
        /// </summary>
        private readonly DatabaseContext context;

        /// <summary>
        ///     The <see cref="NLog" /> logger
        /// </summary>
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        public ProjectManager(DatabaseContext context)
        {
            this.context = context;
        }

        /// <summary>
        ///     Gets all projects contained insed the <see cref="DatabaseContext" />
        /// </summary>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Project" /> as result</returns>
        public async Task<IEnumerable<Project>> GetProjects()
        {
            return await this.context.Projects.ToListAsync();
        }

        /// <summary>
        ///     Creates a new <see cref="Project" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public async Task<EntityOperationResult<Project>> CreateProject(Project project)
        {
            if (this.context.Projects.AsEnumerable().Any(x => x.ProjectName.Equals(project.ProjectName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return EntityOperationResult<Project>.Failed("A project with the same name already exists");
            }

            var operationResult = new EntityOperationResult<Project>(this.context.Add(project), EntityState.Added);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Updates a <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public async Task<EntityOperationResult<Project>> UpdateProject(Project project)
        {
            if (this.context.Projects.AsEnumerable().Any(x => x.ProjectName.Equals(project.ProjectName, StringComparison.InvariantCultureIgnoreCase) 
                                                              && x.Id != project.Id))
            {
                return EntityOperationResult<Project>.Failed("A project with the same name already exists");
            }

            var operationResult = new EntityOperationResult<Project>(this.context.Update(project), EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Delete a <see cref="Project" />
        /// </summary>
        /// <param name="project">The <see cref="Project" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<EntityOperationResult<Project>> DeleteProject(Project project)
        {
            var operationResult = new EntityOperationResult<Project>(this.context.Remove(project), EntityState.Deleted);

            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                operationResult.HandleExpection(exception);
                this.logger.Error(exception.Message);
            }

            return operationResult;
        }

        /// <summary>
        ///     Tries to get a <see cref="Project" /> based on its <see cref="Project.Id" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="Project" /> if existing</returns>
        public async Task<Project> GetProject(Guid id)
        {
            return await this.context.Projects.FindAsync(id);
        }
    }
}
