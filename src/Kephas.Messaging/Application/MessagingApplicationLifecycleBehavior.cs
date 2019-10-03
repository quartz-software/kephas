﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingApplicationLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging application lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Runtime;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A messaging application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class MessagingApplicationLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IMessageBroker messageBroker;
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingApplicationLifecycleBehavior"/>
        /// class.
        /// </summary>
        /// <param name="messageBroker">The message broker.</param>
        /// <param name="eventHub">The event hub.</param>
        public MessagingApplicationLifecycleBehavior(IMessageBroker messageBroker, IEventHub eventHub)
        {
            Requires.NotNull(messageBroker, nameof(messageBroker));
            Requires.NotNull(eventHub, nameof(eventHub));

            this.messageBroker = messageBroker;
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            RuntimeTypeInfo.RegisterFactory(new MessagingTypeInfoFactory());

            if (this.messageBroker is IAsyncInitializable initMessageBroker)
            {
                await initMessageBroker.InitializeAsync(appContext, cancellationToken).PreserveThreadContext();
            }

            if (this.eventHub is IAsyncInitializable initEventHub)
            {
                await initEventHub.InitializeAsync(appContext, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            if (this.messageBroker is IAsyncFinalizable finMessageBroker)
            {
                await finMessageBroker.FinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
            }

            if (this.eventHub is IAsyncFinalizable finEventHub)
            {
                await finEventHub.FinalizeAsync(appContext, cancellationToken).PreserveThreadContext();
            }
        }
    }
}