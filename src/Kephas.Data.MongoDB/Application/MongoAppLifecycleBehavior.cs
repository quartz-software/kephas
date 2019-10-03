﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo application initializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using global::MongoDB.Bson;

    using Kephas.Application;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A MongoDB application lifecycle behavior.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="Id"/> temporary and empty values the ObjectId correspondents.
    /// </remarks>
    public class MongoAppLifecycleBehavior : IAppLifecycleBehavior
    {
        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            var originalIsTemporary = Id.IsTemporary;
            Id.IsTemporary = value =>
            {
                if (value is ObjectId id)
                {
                    return id < ObjectId.Empty;
                }

                return originalIsTemporary(value);
            };

            Id.AddEmptyValue(ObjectId.Empty);

            return TaskHelper.CompletedTask;
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
        public Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }
    }
}