﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// The default application context.
    /// </summary>
    public class AppContext : Context, IAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided,
        ///                               <see cref="AmbientServices.Instance"/> will be considered.</param>
        /// <param name="appManifest">Optional. The application manifest.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        /// <param name="signalShutdown">Function for signaling the application shutdown.</param>
        public AppContext(
            IAmbientServices ambientServices = null,
            IAppManifest appManifest = null,
            IAppArgs appArgs = null,
            Func<IContext, Task<IAppContext>> signalShutdown = null)
            : base(ambientServices)
        {
            this.AppManifest = appManifest ?? this.CompositionContext?.GetExport<IAppManifest>();
            this.AppArgs = appArgs ?? new AppArgs();
            this.SignalShutdown = signalShutdown;
        }

        /// <summary>
        /// Gets the application manifest.
        /// </summary>
        public IAppManifest AppManifest { get; }

        /// <summary>
        /// Gets the application arguments passed typically as command line arguments.
        /// </summary>
        /// <value>
        /// The application arguments.
        /// </value>
        public IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets or sets the application root exception.
        /// </summary>
        /// <value>
        /// The application root exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets a function for signalling the application to shutdown.
        /// </summary>
        /// <value>
        /// The signal shutdown.
        /// </value>
        public Func<IContext, Task<IAppContext>> SignalShutdown { get; }
    }
}