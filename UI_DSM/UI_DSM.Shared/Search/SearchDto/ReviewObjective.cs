// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjective.cs" company="RHEA System S.A.">
// Copyright (c) 2022 RHEA System S.A.
//
// Author: RHEA System S.A.
//
// The current software is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// The current software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software Foundation,
// Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Search.SearchDto
{
	using System;
	using System.Collections.Generic;
	using System.Text.Json.Serialization;
	using UI_DSM.Shared.Search;
	using UI_DSM.Shared.Search.SearchDto;
	using GP.SearchService.SDK.Definitions;

	/// <summary>
	/// The <see cref="ReviewObjective" /> is the <see cref="ISearchDto" /> for the Poco of the same name
	/// </summary>
	[SearchDto(nameof(ReviewObjective))]
	public partial class ReviewObjective : AnnotatableItem
	{

		/// <summary>
		/// Gets or sets the <see cref="Participant"/> <see cref="Author"/>
		/// </summary>
		public Participant Author { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="long"/> <see cref="CreatedOn"/>
		/// </summary>
		public long CreatedOn { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="string"/> <see cref="Title"/>
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="string"/> <see cref="Description"/>
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="string"/> <see cref="ReviewObjectiveKind"/>
		/// </summary>
		public string ReviewObjectiveKind { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="int"/> <see cref="ReviewObjectiveKindNumber"/>
		/// </summary>
		public int ReviewObjectiveKindNumber { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="int"/> <see cref="ReviewObjectiveNumber"/>
		/// </summary>
		public int ReviewObjectiveNumber { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="string"/> <see cref="Status"/>
		/// </summary>
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="List<ReviewTask>"/> <see cref="ReviewTasks"/>
		/// </summary>
		public List<ReviewTask> ReviewTasks { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="List<string>"/> <see cref="RelatedViews"/>
		/// </summary>
		public List<string> RelatedViews { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="List<ReviewCategory>"/> <see cref="ReviewCategories"/>
		/// </summary>
		public List<ReviewCategory> ReviewCategories { get; set; }
	}
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
