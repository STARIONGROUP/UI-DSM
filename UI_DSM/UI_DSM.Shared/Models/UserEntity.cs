// --------------------------------------------------------------------------------------------------------
// <copyright file="UserEntity.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Shared.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     <see cref="Entity" /> to have a link to a <see cref="User" />
    /// </summary>
    [Table(nameof(UserEntity))]
    public class UserEntity : Entity
    {
        /// <summary>
        ///     Inilializes a new <see cref="Entity" />
        /// </summary>
        /// <param name="id">The <see cref="Guid" /> of the <see cref="Entity" /></param>
        public UserEntity(Guid id) : base(id)
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="Entity" />
        /// </summary>
        public UserEntity()
        {
        }

        /// <summary>
        ///     The <see cref="User" />
        /// </summary>
        public User User { get; set; }

        /// <summary>
        ///     The <see cref="User" /> id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     The user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Value indicating if the user is admin or not
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        ///     Instantiate a <see cref="EntityDto" /> from a <see cref="Entity" />
        /// </summary>
        /// <returns>A new <see cref="EntityDto" /></returns>
        public override EntityDto ToDto()
        {
            return new UserDto(this.Id)
            {
                UserName = this.UserName,
                IsAdmin = this.IsAdmin
            };
        }

        /// <summary>
        ///     Resolve the properties of the current <see cref="Entity" /> from its <see cref="EntityDto" /> counter-part
        /// </summary>
        /// <param name="entityDto">The source <see cref="EntityDto" /></param>
        /// <param name="resolvedEntity">A <see cref="Dictionary{TKey,TValue}" /> of all <see cref="Entity" /></param>
        public override void ResolveProperties(EntityDto entityDto, Dictionary<Guid, Entity> resolvedEntity)
        {
            if (entityDto is not UserDto userDto)
            {
                throw new InvalidOperationException($"The DTO {entityDto.GetType()} does not match with the current UserEntity POCO");
            }

            this.UserName = userDto.UserName;
            this.IsAdmin = userDto.IsAdmin;
        }
    }
}
