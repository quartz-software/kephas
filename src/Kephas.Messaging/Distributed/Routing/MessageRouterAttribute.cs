﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRouterAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message router attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using Kephas.Messaging.Distributed.Routing.Composition;
    using Kephas.Services;

    /// <summary>
    /// Attribute for message router.
    /// </summary>
    public class MessageRouterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the receiver URL regular expression.
        /// </summary>
        /// <value>
        /// The receiver URL regular expression.
        /// </value>
        [MetadataValue(nameof(MessageRouterMetadata.ReceiverUrlRegex))]
        public string ReceiverUrlRegex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this router is the fallback router.
        /// </summary>
        /// <value>
        /// True if this router is fallback, false if not.
        /// </value>
        [MetadataValue(nameof(MessageRouterMetadata.IsFallback))]
        public bool IsFallback { get; set; }
    }
}