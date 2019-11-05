﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Testing.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text;
    using global::Autofac;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Composition.Autofac.Metadata;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacCompositionTestBase : TestBase
    {
        public virtual ContainerBuilder WithEmptyConfiguration()
        {
            var configuration = new ContainerBuilder();
            return configuration;
        }

        public virtual ContainerBuilder WithExportProviders(ContainerBuilder configuration)
        {
            configuration.RegisterSource(new ExportFactoryRegistrationSource());
            configuration.RegisterSource(new ExportFactoryWithMetadataRegistrationSource());
            return configuration;
        }

        public virtual AutofacCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager logManager = null, IAppRuntime appRuntime = null)
        {
            var log = new StringBuilder();
            logManager = logManager ?? new DebugLogManager((logger, level, message, ex) => log.AppendLine($"[{logger}] [{level}] {message}{ex}"));
            appRuntime = appRuntime ?? new StaticAppRuntime(
                             logManager: logManager,
                             defaultAssemblyFilter: a => !a.IsSystemAssembly() && !a.FullName.StartsWith("NUnit") && !a.FullName.StartsWith("xunit") && !a.FullName.StartsWith("JetBrains"));

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .Register(logManager)
                .Register(appRuntime)
                .Register(log);
            return new AutofacCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return CreateContainer(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<AutofacCompositionContainerBuilder> config = null)
        {
            ambientServices = ambientServices ?? new AmbientServices();
            var containerBuilder = WithContainerBuilder(ambientServices)
                    .WithAssemblies(GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            ambientServices.Register(container);
            return container;
        }

        public ICompositionContext CreateContainerWithBuilder(Action<AutofacCompositionContainerBuilder> config = null)
        {
            var builder = WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
                           typeof(AutofacCompositionContainer).GetTypeInfo().Assembly, /* Kephas.Composition.Autofac */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}