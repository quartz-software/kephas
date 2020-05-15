﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Scheduling.Reflection;

namespace Kephas.Scheduling.Jobs
{
    using System.Threading.Tasks;

    using Kephas.Operations;

    /// <summary>
    /// Encapsulates the result of a job.
    /// </summary>
    public class JobResult : OperationResult<object?>, IJobResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        public JobResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="jobOperation">The job operation.</param>
        public JobResult(object jobId, Task<object?> jobOperation)
            : base(jobOperation)
        {
            this.JobId = jobId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobResult"/> class.
        /// </summary>
        /// <param name="triggerId">The trigger identifier.</param>
        public JobResult(object triggerId)
        {
            this.TriggerId = triggerId;
        }

        /// <summary>
        /// Gets or sets the ID of the job information.
        /// </summary>
        public object? JobInfoId { get; set; }

        /// <summary>
        /// Gets or sets the job information.
        /// </summary>
        public IJobInfo? JobInfo { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the job.
        /// </summary>
        /// <value>
        /// The identifier of the job.
        /// </value>
        public object? JobId { get; set; }

        /// <summary>
        /// Gets or sets the job.
        /// </summary>
        /// <value>
        /// The job.
        /// </value>
        public IJob? Job { get; set; }

        /// <summary>
        /// Gets or sets the trigger identifier.
        /// </summary>
        public object? TriggerId { get; set; }

        /// <summary>
        /// Gets or sets the time when the job started.
        /// </summary>
        public DateTimeOffset? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the time when the job ended.
        /// </summary>
        public DateTimeOffset? EndedAt { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public override TimeSpan Elapsed
        {
            get
            {
                if (base.Elapsed == TimeSpan.Zero && this.StartedAt.HasValue)
                {
                        return this.EndedAt.HasValue
                            ? this.EndedAt.Value - this.StartedAt.Value
                            : DateTimeOffset.Now - this.StartedAt.Value;
                }

                return base.Elapsed;
            }
        }
    }
}