﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEventHub.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEventHub interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Composition;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Contract for the shared application service handling in-process publish/subscribe .
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEventHub
    {
        /// <summary>
        /// Publishes the event asynchronously to its subscribers.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task PublishAsync(object @event, IContext context, CancellationToken cancellationToken = default);

        /// <summary>
        /// Subscribes to the event(s) matching the criteria.
        /// </summary>
        /// <param name="match">Specifies the match criteria.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        IEventSubscription Subscribe(IMessageMatch match, Func<object, IContext, CancellationToken, Task> callback);
    }

    /// <summary>
    /// Extension methods for <see cref="IEventHub"/>.
    /// </summary>
    public static class EventHubExtensions
    {
        /// <summary>
        /// Subscribes to the event with the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="messageTypeMatching">Optional. The message type matching.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(
            this IEventHub eventHub,
            Func<TEvent, IContext, CancellationToken, Task> callback,
            MessageTypeMatching messageTypeMatching = MessageTypeMatching.Type)
            where TEvent : class
        {
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(callback, nameof(callback));

            return eventHub.Subscribe(
                new MessageMatch
                {
                    MessageType = typeof(TEvent),
                    MessageTypeMatching = messageTypeMatching,
                },
                (e, ctx, token) => callback((TEvent)e, ctx, token));
        }

        /// <summary>
        /// Subscribes to the event with the provided type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="eventHub">The eventHub to act on.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="messageTypeMatching">Optional. The message type matching.</param>
        /// <returns>
        /// An IEventSubscription.
        /// </returns>
        public static IEventSubscription Subscribe<TEvent>(
            this IEventHub eventHub,
            Action<TEvent, IContext> callback,
            MessageTypeMatching messageTypeMatching = MessageTypeMatching.Type)
            where TEvent : class
        {
            Requires.NotNull(eventHub, nameof(eventHub));
            Requires.NotNull(callback, nameof(callback));

            return eventHub.Subscribe(
                new MessageMatch
                {
                    MessageType = typeof(TEvent),
                    MessageTypeMatching = messageTypeMatching,
                },
                (e, ctx, token) =>
                {
                    callback((TEvent)e, ctx);
                    return TaskHelper.CompletedTask;
                });
        }
    }
}