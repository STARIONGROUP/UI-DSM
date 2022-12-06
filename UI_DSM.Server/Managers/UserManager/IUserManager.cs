// --------------------------------------------------------------------------------------------------------
// <copyright file="IUserManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.UserManager
{
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="UserManager" />
    /// </summary>
    public interface IUserManager : IEntityManager<UserEntity>
    {
        /// <summary>
        ///     Register a new <see cref="User" />
        /// </summary>
        /// <param name="user">The <see cref="User" /> to create</param>
        /// <param name="password">The <see cref="User" /> password</param>
        /// <returns>A <see cref="Task" /> with a <see cref="EntityOperationResult{TEntity}" /> as result</returns>
        Task<EntityOperationResult<UserEntity>> RegisterUser(User user, string password);

        /// <summary>
        ///     Gets a <see cref="UserEntity" /> by its name
        /// </summary>
        /// <param name="userName">The name of the <see cref="UserEntity" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="UserEntity" /> if found</returns>
        Task<UserEntity> GetUserByName(string userName);

        /// <summary>
        ///     Gets a collection of <see cref="UserEntity" /> by predicate
        /// </summary>
        /// <param name="predicate">The <see cref="Predicate{T}" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="UserEntity" /></returns>
        Task<IEnumerable<UserEntity>> GetUsers(Func<UserEntity, bool> predicate);
    }
}
