﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdGeneratorSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the identifier generator settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    /// <summary>
    /// Settings for the <see cref="DefaultIdGenerator"/>.
    /// </summary>
    public class IdGeneratorSettings
    {
        /// <summary>
        /// Gets or sets the start epoch for the timestamp part of an ID - 2015-06-01.
        /// </summary>
        public DateTimeOffset StartEpoch { get; set; } = new DateTimeOffset(new DateTime(2018, 8, 1), TimeSpan.Zero);

        /// <summary>
        /// Gets or sets the length of the namespace identifier bits.
        /// </summary>
        /// <value>
        /// The length of the namespace identifier bits.
        /// </value>
        public int NamespaceIdentifierBitLength { get; set; } = 3;

        /// <summary>
        /// Gets or sets the length of the discriminator part bits.
        /// </summary>
        /// <value>
        /// The length of the discriminator part bits.
        /// </value>
        public int DiscriminatorBitLength { get; set; } = 7;
    }
}