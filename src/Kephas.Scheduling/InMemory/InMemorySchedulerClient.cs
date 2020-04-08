﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryJobSchedulerClient.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory job scheduler client class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Scheduling.Jobs;

namespace Kephas.Scheduling.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// An in memory job scheduler client.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemorySchedulerClient : Loggable, ISchedulerClient
    {
        private readonly IEventHub eventHub;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySchedulerClient"/> class.
        /// </summary>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="logManager">The log manager.</param>
        public InMemorySchedulerClient(
            IEventHub eventHub,
            IContextFactory contextFactory,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.eventHub = eventHub;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Starts a new job asynchronously.
        /// <para>
        /// The job information provided may be either an ID, a qualified name, or a
        /// <see cref="IJobInfo"/>.
        /// </para>
        /// </summary>
        /// <param name="jobInfo">Information describing the job.</param>
        /// <param name="target">Target for the.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public async Task<IJobResult> EnqueueAsync(
            object jobInfo,
            object? target,
            IExpando? arguments,
            Action<IActivityContext>? options = null,
            CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new EnqueueEvent
            {
                JobInfo = jobInfo,
                TriggerId = Guid.NewGuid(),
                Target = target,
                Arguments = arguments,
                Options = options,
            };
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult(enqueueEvent.TriggerId)
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }

        /// <summary>
        /// Cancels the job asynchronously.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public async Task<IJobResult> CancelJobAsync(object jobId, CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new CancelJobEvent
            {
                JobId = jobId,
            };
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult { JobId = jobId }
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }

        /// <summary>
        /// Cancels the trigger asynchronously.
        /// </summary>
        /// <param name="triggerId">The trigger identifier.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        public async Task<IJobResult> CancelTriggerAsync(object triggerId, CancellationToken cancellationToken = default)
        {
            var enqueueEvent = new CancelTriggerEvent
            {
                TriggerId = triggerId,
            };
            await this.eventHub.PublishAsync(
                enqueueEvent,
                this.contextFactory.CreateContext<Context>(),
                cancellationToken);

            return new JobResult(triggerId)
                .Complete(TimeSpan.Zero, OperationState.InProgress);
        }
    }
}