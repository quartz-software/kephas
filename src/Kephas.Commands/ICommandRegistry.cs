﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICommandRegistry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for the service registering command types.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ICommandRegistry
    {
        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <param name="commandPattern">Optional. A pattern specifying the command types to retrieve.</param>
        /// <returns>
        /// The command types.
        /// </returns>
        IEnumerable<ITypeInfo> GetCommandTypes(string? commandPattern = null);

        /// <summary>
        /// Resolves the command type.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        ITypeInfo ResolveCommandType(string command);
    }
}
