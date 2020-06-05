﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSettingsHandlerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Runtime;
using Kephas.Threading.Tasks;

namespace Kephas.Core.Endpoints.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Serialization;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateSettingsHandlerTest
    {
        [Test]
        public async Task ProcessAsync_instance()
        {
            var settings = new CoreSettings();
            var config = Substitute.For<IConfiguration<CoreSettings>>();
            config.UpdateSettingsAsync(Arg.Any<CoreSettings>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var success = ci.Arg<CoreSettings>().Task.DefaultTimeout == TimeSpan.FromMinutes(5);
                    return Task.FromResult<IOperationResult<bool>>(
                        new OperationResult<bool>(success)
                        .Complete(operationState: success ? OperationState.Completed : OperationState.Warning));
                });
            var container = Substitute.For<ICompositionContext>();
            container.GetExport(typeof(IConfiguration<CoreSettings>))
                .Returns(config);
            var typeResolver = new DefaultTypeResolver(() => new List<Assembly> { typeof(CoreSettings).Assembly });

            var handler = new UpdateSettingsHandler(container, typeResolver, Substitute.For<ISerializationService>(), new RuntimeTypeRegistry());
            var result = await handler.ProcessAsync(
                new UpdateSettingsMessage { Settings = new CoreSettings { Task = new TaskSettings { DefaultTimeout = TimeSpan.FromMinutes(5) } } },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(SeverityLevel.Info, result.Severity);
        }

        [Test]
        public async Task ProcessAsync_string()
        {
            var settings = new CoreSettings();
            var config = Substitute.For<IConfiguration<CoreSettings>>();
            config.UpdateSettingsAsync(Arg.Any<CoreSettings>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var success = ci.Arg<CoreSettings>().Task.DefaultTimeout == TimeSpan.FromMinutes(5);
                    return Task.FromResult<IOperationResult<bool>>(
                        new OperationResult<bool>(success)
                            .Complete(operationState: success ? OperationState.Completed : OperationState.Warning));
                });
            var container = Substitute.For<ICompositionContext>();
            container.GetExport(typeof(IConfiguration<CoreSettings>))
                .Returns(config);
            var typeResolver = new DefaultTypeResolver(() => new List<Assembly> { typeof(CoreSettings).Assembly });

            var settingsString = @"{""task"": {""defaultTimeout"": ""0:5:0""} }";
            var serializationService = Substitute.For<ISerializationService>();
#if NETCORE3_1
            serializationService.Deserialize(settingsString, Arg.Any<Action<ISerializationContext>>())
                .Returns(ci => new CoreSettings { Task = new TaskSettings {DefaultTimeout = TimeSpan.FromMinutes(5) } });
#else
            serializationService.DeserializeAsync(settingsString, Arg.Any<Action<ISerializationContext>>(),
                    Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<object?>(
                    new CoreSettings { Task = new TaskSettings {DefaultTimeout = TimeSpan.FromMinutes(5) } }));
#endif
            var handler = new UpdateSettingsHandler(container, typeResolver, serializationService, new RuntimeTypeRegistry());
            var result = await handler.ProcessAsync(
                new UpdateSettingsMessage { SettingsType = "core", Settings = settingsString },
                Substitute.For<IMessagingContext>(),
                default);

            Assert.AreEqual(SeverityLevel.Info, result.Severity);
        }
    }
}