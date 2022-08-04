// --------------------------------------------------------------------------------------------------------
// <copyright file="JwtParser.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Features
{
    using System.Security.Claims;
    using System.Text.Json;

    /// <summary>
    ///     This utility class provide methods to help to parse a JWT
    /// </summary>
    public static class JwtParser
    {
        /// <summary>
        ///     Provides a collection of <see cref="Claim" /> contained in the JWT
        /// </summary>
        /// <param name="jwt">The JWT</param>
        /// <returns>A collection of <see cref="Claim" /></returns>
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        /// <summary>
        ///     Parse a based 64 string
        /// </summary>
        /// <param name="base64">The <see cref="string" /></param>
        /// <returns>An <see cref="byte" /> array</returns>
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
