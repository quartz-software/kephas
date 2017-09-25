﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBrokeredMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IMessageEnvelope interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    using Kephas.Data;

    /// <summary>
    /// Contract interface for brokered messages.
    /// </summary>
    public interface IBrokeredMessage : IIdentifiable, IMessage
    {
        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        /// <value>
        /// The message sender.
        /// </value>
        IMessageSender Sender { get; }

        /// <summary>
        /// Gets the message to send.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IMessage Message { get; }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        IMessageChannel Channel { get; }

        /// <summary>
        /// Gets a value indicating whether this message is one way.
        /// </summary>
        /// <value>
        /// True if this message is one way, false if not.
        /// </value>
        bool IsOneWay { get; }

        /// <summary>
        /// Gets the timeout when waiting for responses.
        /// </summary>
        /// <value>
        /// The response timeout.
        /// </value>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the identifier of the message which this message is a reply to.
        /// </summary>
        /// <value>
        /// The identifier of the reply to message.
        /// </value>
        object ReplyToMessageId { get; }
    }
}