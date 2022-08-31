// --------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Extensions
{
    using Npgsql;

    /// <summary>
    ///     Extension class for <see cref="Exception" />
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        ///     Check if the <see cref="Exception" /> results after a unique constraint violation
        /// </summary>
        /// <param name="ex">The <see cref="Exception" /></param>
        /// <returns>True if the <see cref="Exception" /> is a unique constraint violation</returns>
        public static bool IsUniqueConstraintViolation(Exception ex)
        {
            var innermost = GetInnermostException(ex);
            return innermost is PostgresException { SqlState: "23505" };
        }

        /// <summary>
        ///     Gets the most inner exception inside an <see cref="Exception" />
        /// </summary>
        /// <param name="ex">The <see cref="Exception" /></param>
        /// <returns>The most inner <see cref="Exception" /></returns>
        private static Exception GetInnermostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}
