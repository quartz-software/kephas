﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security
{
    using Kephas.Services;

    /// <summary>
    /// Interface for authorization context.
    /// </summary>
    public interface IAuthorizationContext : IContext
    {
        /// <summary>
        /// Gets a value indicating whether to throw on failure.
        /// </summary>
        /// <value>
        /// True if throw on failure, false if not.
        /// </value>
        bool ThrowOnFailure { get; }
    }
}