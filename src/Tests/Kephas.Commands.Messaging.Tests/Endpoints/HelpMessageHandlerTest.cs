﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpMessageHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the help message handler test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ComponentModel.DataAnnotations;

namespace Kephas.Commands.Messaging.Tests.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Commands.Messaging.Endpoints;
    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Services.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HelpMessageHandlerTest
    {
        [Test]
        public async Task ProcessAsync_nullable_param()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var registry = Substitute.For<ICommandRegistry>();
            registry.GetCommandTypes(Arg.Any<string>()).Returns(
                ci => new List<IOperationInfo>()
                    {
                        // new MessageOperationInfo(typeof(HelpMessage).AsRuntimeTypeInfo(), lazyMessageProcessor),
                        new MessageOperationInfo(typeof(NullableParamMessage).AsRuntimeTypeInfo(), lazyMessageProcessor),
                    });

            var handler = new HelpMessageHandler(
                new List<Lazy<ICommandRegistry, AppServiceMetadata>>
                {
                    new Lazy<ICommandRegistry, AppServiceMetadata>(
                        () => registry,
                        new AppServiceMetadata()),
                });

            var helpResponse = await handler.ProcessAsync(new HelpMessage(), Substitute.For<IMessagingContext>(), default);
            var command = helpResponse.Command as string;
            Assert.AreEqual("NullableParam", command);
            Assert.AreEqual(1, helpResponse.Parameters.Length);
            Assert.AreEqual("StartTime (System.DateTime?): ", helpResponse.Parameters[0]);
        }

        [Test]
        public async Task ProcessAsync_long_param_name()
        {
            var lazyMessageProcessor = new Lazy<IMessageProcessor>(() => Substitute.For<IMessageProcessor>());
            var registry = Substitute.For<ICommandRegistry>();
            registry.GetCommandTypes(Arg.Any<string>()).Returns(
                ci => new List<IOperationInfo>()
                {
                    // new MessageOperationInfo(typeof(HelpMessage).AsRuntimeTypeInfo(), lazyMessageProcessor),
                    new MessageOperationInfo(typeof(LongParamMessage).AsRuntimeTypeInfo(), lazyMessageProcessor),
                });

            var handler = new HelpMessageHandler(
                new List<Lazy<ICommandRegistry, AppServiceMetadata>>
                {
                    new Lazy<ICommandRegistry, AppServiceMetadata>(
                        () => registry,
                        new AppServiceMetadata()),
                });

            var helpResponse = await handler.ProcessAsync(new HelpMessage(), Substitute.For<IMessagingContext>(), default);
            var command = helpResponse.Command as string;
            Assert.AreEqual("LongParam", command);
            Assert.AreEqual(1, helpResponse.Parameters.Length);
            Assert.AreEqual("IncludePrerelease/pre (System.Boolean): Includes prerelease.", helpResponse.Parameters[0]);
        }

        public class NullableParamMessage : IMessage
        {
            public DateTime? StartTime { get; set; }
        }

        public class LongParamMessage : IMessage
        {
            [DisplayInfo(ShortName = "pre", Description = "Includes prerelease.")]
            public bool IncludePrerelease { get; set; }
        }
    }
}