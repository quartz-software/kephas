﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.Mef.ExportProviders;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Configuration;
    using Kephas.Hosting;
    using Kephas.Logging;

    using NUnit.Framework;

    using Telerik.JustMock;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase
    {
        [OneTimeSetUp]
        public void TestSetup()
        {
            this.OnTestSetup();
        }

        public virtual void OnTestSetup()
        {
        }

        public virtual ContainerConfiguration WithEmptyConfiguration()
        {
            return new ContainerConfiguration();
        }

        public virtual ContainerConfiguration WithExportProviders(ContainerConfiguration configuration)
        {
            configuration.WithProvider(new ExportFactoryExportDescriptorProvider());
            configuration.WithProvider(new ExportFactoryWithMetadataExportDescriptorProvider());
            return configuration;
        }

        public virtual MefCompositionContainerBuilder WithContainerBuilder(ILogManager logManager = null, IConfigurationManager configurationManager = null, IHostingEnvironment hostingEnvironment = null)
        {
            logManager = logManager ?? Mock.Create<ILogManager>();
            configurationManager = configurationManager ?? Mock.Create<IConfigurationManager>();
            hostingEnvironment = hostingEnvironment ?? Mock.Create<IHostingEnvironment>();

            return new MefCompositionContainerBuilder(logManager, configurationManager, hostingEnvironment);
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer((IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies)
        {
            return
                this.WithContainerBuilder()
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).Assembly,     /* Kephas.Core*/
                           typeof(MefCompositionContainer).Assembly, /* Kephas.Composition.Mef */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}