﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncBehaviorRuleBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for asynchronous behavior rules.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Runtime;

    /// <summary>
    /// Base class for asynchronous behavior rules.
    /// </summary>
    /// <typeparam name="TContext">Type of the context.</typeparam>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public abstract class AsyncBehaviorRuleBase<TContext, TValue> : BehaviorRuleFlowControlBase, IAsyncBehaviorRule<TContext, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncBehaviorRuleBase{TContext, TValue}"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        protected AsyncBehaviorRuleBase(IRuntimeTypeRegistry typeRegistry, ILogManager? logManager = null)
            : base(typeRegistry, logManager)
        {
        }

        /// <summary>
        /// Gets a value asynchronously indicating whether the rule may be applied or not.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of a value indicating whether the rule may be applied or not.
        /// </returns>
        public virtual Task<bool> CanApplyAsync(TContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        Task<IBehaviorValue> IAsyncBehaviorRule<TContext>.GetValueAsync(TContext context, CancellationToken cancellationToken)
        {
            // do not use await, to be able to use it both on the client and the server.
            return this.GetValueAsync(context, cancellationToken).ContinueWith(t => (IBehaviorValue)t.Result, cancellationToken);
        }

        /// <summary>
        /// Gets the behavior value asynchronously.
        /// </summary>
        /// <param name="context">          The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the behavior value.
        /// </returns>
        public abstract Task<IBehaviorValue<TValue>> GetValueAsync(TContext context, CancellationToken cancellationToken = default);
    }
}