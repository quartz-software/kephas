﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Messaging.Distributed;
    using Kephas.Security.Authentication;
    using Kephas.Services;
    using Kephas.Testing.Application;

    using NSubstitute;

    public class MefMessagingTestBase : MefApplicationTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).GetTypeInfo().Assembly); /* Kephas.Messaging */
            return base.CreateContainer(ambientServices, assemblyList, parts, config);
        }

        protected virtual ICompositionContext CreateMessagingContainerMock()
        {
            var container = Substitute.For<ICompositionContext>();

            container.GetExport(typeof(IExportFactoryImporter<IContextFactory>), Arg.Any<string>())
                .Returns(ci =>
                    new ExportFactoryImporter<IContextFactory>(
                        new ExportFactory<IContextFactory>(
                            () =>
                            {
                                return this.CreateContextFactoryMock(args =>
                                    new DispatchingContext(
                                        container,
                                        Substitute.For<IAppRuntime>(),
                                        Substitute.For<IAuthenticationService>(),
                                        args.Length > 0 ? args[0] : null));
                            })));

            return container;
        }
    }
}
