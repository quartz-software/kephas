﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the serilog manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging.Serilog
{
    using System;
    using System.Collections.Concurrent;

    using global::Serilog;
    using global::Serilog.Core;
    using global::Serilog.Events;

    /// <summary>
    /// The Serilog log manager.
    /// </summary>
    public class SerilogManager : ILogManager, IDisposable
    {
        private readonly LoggerConfiguration configuration;
        private readonly LoggingLevelSwitch levelSwitch;
        private readonly ConcurrentDictionary<string, global::Kephas.Logging.ILogger> loggers = new ConcurrentDictionary<string, global::Kephas.Logging.ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogManager"/> class.
        /// </summary>
        /// <param name="configuration">Optional. The configuration.</param>
        /// <param name="levelSwitch">Optional. The logging level switch.</param>
        public SerilogManager(LoggerConfiguration? configuration = null, LoggingLevelSwitch? levelSwitch = null)
        {
            this.configuration = configuration ?? new LoggerConfiguration();
            this.levelSwitch = levelSwitch ?? new LoggingLevelSwitch();
            this.configuration.MinimumLevel.ControlledBy(this.levelSwitch);
            this.RootLogger = this.configuration.CreateLogger();
        }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public LogLevel MinimumLevel
        {
            get => this.levelSwitch.MinimumLevel.ToLogLevel();
            set => this.levelSwitch.MinimumLevel = value.ToLogEventLevel();
        }

        /// <summary>
        /// Gets the root logger.
        /// </summary>
        /// <value>
        /// The root logger.
        /// </value>
        protected Logger RootLogger { get; }

        /// <summary>
        /// Gets the logger with the provided name.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger for the provided name.</returns>
        public global::Kephas.Logging.ILogger GetLogger(string loggerName)
        {
            return this.loggers.GetOrAdd(loggerName, this.CreateLogger);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.loggers.Clear();
            this.RootLogger.Dispose();
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <returns>A logger with the provided name.</returns>
        protected virtual global::Kephas.Logging.ILogger CreateLogger(string loggerName)
        {
            var logger = new SerilogLogger(loggerName, this.RootLogger);
            return logger;
        }
    }
}