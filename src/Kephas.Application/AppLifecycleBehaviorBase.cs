﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLifecycleBehaviorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application lifecycle behavior base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application.Resources;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Text;

    /// <summary>
    /// Base class for application lifecycle behaviors.
    /// </summary>
    public abstract class AppLifecycleBehaviorBase : Loggable, IAppLifecycleBehavior
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        Task IAppLifecycleBehavior.BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            return this.BeforeAppInitializeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        public virtual Task BeforeAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IAppLifecycleBehavior.AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            return this.AfterAppInitializeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IAppLifecycleBehavior.BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            return this.BeforeAppFinalizeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <remarks>
        /// To interrupt finalization, simply throw any appropriate exception.
        /// Caution! Interrupting the finalization may cause the application to remain in an undefined state.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task BeforeAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IAppLifecycleBehavior.AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken)
        {
            return this.AfterAppFinalizeAsync(this.GetAppContext(appContext), cancellationToken);
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task AfterAppFinalizeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IAppContext GetAppContext(IContext context)
        {
            return context is IAppContext appContext
                ? appContext
                : throw new ApplicationException(Strings.AppLifecycleBehaviorBase_MismatchedAppContext_Exception.FormatWith(nameof(IAppContext)));
        }
    }
}