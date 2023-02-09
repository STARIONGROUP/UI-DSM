// --------------------------------------------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Extensions
{
	using System.Text;

	/// <summary>
	///     Extension class for <see cref="string" />
	/// </summary>
	public static class StringExtension
	{
		/// <summary>
		///     Add a blank space before each uppercase inside the text
		/// </summary>
		/// <param name="text">The current text</param>
		/// <returns>The text with additional uppercase</returns>
		public static string AddSpaceBeforeUpperCase(this string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return "";
			}

			var newText = new StringBuilder();
			newText.Append(text[0]);

			for (var charachterIndex = 1; charachterIndex < text.Length; charachterIndex++)
			{
				if (char.IsUpper(text[charachterIndex]) && text[charachterIndex - 1] != ' ')
				{
					newText.Append(' ');
				}

				newText.Append(text[charachterIndex]);
			}

			return newText.ToString();
		}
	}
}
