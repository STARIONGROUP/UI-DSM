// --------------------------------------------------------------------------------------------------------
// <copyright file="IUserService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.Administration.UserService
{
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;

    /// <summary>
    ///     Interface definition for <see cref="UserService" />
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        ///     Provide a collection of registered <see cref="UserDto" />
        /// </summary>
        /// <returns>A task where the result is a collection of <see cref="UserDto" /></returns>
        Task<List<UserDto>> GetUsers();

        /// <summary>
        ///     Register a new user based on <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="newUser">The <see cref="RegistrationDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the request</returns>
        Task<RegistrationResponseDto> RegisterUser(RegistrationDto newUser);

        /// <summary>
        ///     Delete a registered user
        /// </summary>
        /// <param name="userToDelete">The user to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        Task<RequestResponseDto> DeleteUser(UserDto userToDelete);
    }
}
