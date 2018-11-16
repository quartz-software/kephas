﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureAuthenticatedBrokeredMessageProcessingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the ensure authenticated brokered message behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging.Behaviors;
    using Kephas.Messaging.Behaviors.AttributedModel;
    using Kephas.Messaging.Composition;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A distributed message processing behavior ensuring that the bearer token is transformed to an .
    /// </summary>
    [MessageProcessingBehavior(MessageTypeMatching.TypeOrHierarchy)]
    [ProcessingPriority(Priority.Highest)]
    public class EnsureAuthenticatedBrokeredMessageProcessingBehavior : MessageProcessingBehaviorBase<IBrokeredMessage>
    {
        /// <summary>
        /// The authentication service.
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="EnsureAuthenticatedBrokeredMessageProcessingBehavior"/>
        /// class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public EnsureAuthenticatedBrokeredMessageProcessingBehavior(IAuthenticationService authenticationService)
        {
            Requires.NotNull(authenticationService, nameof(authenticationService));

            this.authenticationService = authenticationService;
        }

        /// <summary>
        /// Interception called before invoking the handler to process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public override async Task BeforeProcessAsync(IBrokeredMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.BearerToken) || context.Identity != null)
            {
                return;
            }

            var identity = await this.authenticationService.GetIdentityAsync(message.BearerToken, context, token)
                               .PreserveThreadContext();
            context.Identity = identity;
        }
    }
}