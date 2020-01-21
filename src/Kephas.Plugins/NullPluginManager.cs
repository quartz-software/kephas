﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPluginManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null plugin manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;

    /// <summary>
    /// The null plugin manager.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullPluginManager : IPluginManager
    {
        /// <summary>
        /// Disables the plugin asynchronously if the plugin was previously enabled.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public Task<IOperationResult> DisablePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }

        /// <summary>
        /// Enables the plugin asynchronously if the plugin was previously disabled.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public Task<IOperationResult> EnablePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }

        /// <summary>
        /// Gets available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public Task<IEnumerable<IPluginInfo>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<IPluginInfo>>(new IPluginInfo[0]);
        }

        /// <summary>
        /// Gets the plugins in this collection.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the plugins in this collection.
        /// </returns>
        public IEnumerable<IPlugin> GetInstalledPlugins()
        {
            yield break;
        }

        /// <summary>
        /// Initializes the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the initialize operation result.
        /// </returns>
        public Task<IOperationResult> InitializePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public Task<IOperationResult> InstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }

        /// <summary>
        /// Uninitializes the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninitialize operation result.
        /// </returns>
        public Task<IOperationResult> UninitializePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall plugin.
        /// </returns>
        public Task<IOperationResult> UninstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IOperationResult>(new OperationResult().MergeException(new NotSupportedException()));
        }
    }
}