﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default application context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// The default application context.
    /// </summary>
    public class AppContext : Context, IAppContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional). If not provided,
        ///                               <see cref="AmbientServices.Instance"/> will be considered.</param>
        /// <param name="appManifest">The application manifest (optional).</param>
        /// <param name="appArgs">The application arguments (optional).</param>
        public AppContext(IAmbientServices ambientServices = null, IAppManifest appManifest = null, string[] appArgs = null)
            : base(ambientServices)
        {
            this.AppManifest = appManifest ?? this.AmbientServices?.CompositionContainer.GetExport<IAppManifest>();
            this.AppArgs = appArgs;
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
        public string[] AppArgs { get; }
    }
}