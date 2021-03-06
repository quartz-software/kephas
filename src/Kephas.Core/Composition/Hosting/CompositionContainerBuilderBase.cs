﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for composition container builders.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Composition.Configuration;
    using Kephas.Composition.Conventions;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Reflection;

    /// <summary>
    /// Base class for composition container builders.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public abstract class CompositionContainerBuilderBase<TBuilder> : ICompositionContainerBuilder
        where TBuilder : CompositionContainerBuilderBase<TBuilder>
    {
        private HashSet<Assembly> compositionAssemblies;
        private HashSet<Assembly> conventionAssemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerBuilderBase{TBuilder}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Must register the ambient services in the composition context.")]
        protected CompositionContainerBuilderBase(ICompositionRegistrationContext context)
        {
            Requires.NotNull(context, nameof(context));
            var ambientServices = context.AmbientServices;
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.RegistrationContext = context;

            this.LogManager = ambientServices.LogManager;
            this.AssertRequiredService(this.LogManager);

            this.AppRuntime = ambientServices.AppRuntime;
            this.AssertRequiredService(this.AppRuntime);

            this.TypeLoader = ambientServices.TypeLoader;
            this.AssertRequiredService(this.TypeLoader);

            this.Logger = this.LogManager.GetLogger(this.GetType());

            context.AppServiceInfoProviders = context.AppServiceInfoProviders == null
                ? new List<IAppServiceInfoProvider> { this.Registry }
                : new List<IAppServiceInfoProvider>(context.AppServiceInfoProviders) { this.Registry };
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        protected internal ILogManager LogManager { get; }

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        /// <value>
        /// The type loader.
        /// </value>
        protected internal ITypeLoader TypeLoader { get; }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected internal IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the conventions builder.
        /// </summary>
        /// <value>
        /// The conventions builder.
        /// </value>
        protected IConventionsBuilder ConventionsBuilder { get; private set; }

        /// <summary>
        /// Gets the composition parts.
        /// </summary>
        /// <value>
        /// The composition parts.
        /// </value>
        protected HashSet<Type> CompositionParts { get; private set; }

        /// <summary>
        /// Gets the <see cref="IAppServiceInfo"/> serviceRegistry.
        /// </summary>
        /// <value>
        /// The serviceRegistry.
        /// </value>
        protected AppServiceInfoRegistry Registry { get; } = new AppServiceInfoRegistry();

        /// <summary>
        /// Gets the registration context.
        /// </summary>
        protected ICompositionRegistrationContext RegistrationContext { get; }

        /// <summary>
        /// Adds the assemblies containing the conventions.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithConventionAssemblies(IEnumerable<Assembly> assemblies)
        {
            Requires.NotNull(assemblies, nameof(assemblies));

            if (this.conventionAssemblies == null)
            {
                this.conventionAssemblies = new HashSet<Assembly>(assemblies);
            }
            else
            {
                this.conventionAssemblies.AddRange(assemblies);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assembly containing the conventions.
        /// </summary>
        /// <param name="assembly">The convention assembly.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assembly is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithConventionAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.WithConventionAssemblies(new[] { assembly });
        }

        /// <summary>
        /// Sets the composition conventions.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>This builder.</returns>
        public virtual TBuilder WithConventions(IConventionsBuilder conventions)
        {
            Requires.NotNull(conventions, nameof(conventions));

            this.ConventionsBuilder = conventions;

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assemblies containing the composition parts.
        /// </summary>
        /// <param name="assemblies">The composition assemblies.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assemblies are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssemblies(IEnumerable<Assembly> assemblies)
        {
            Requires.NotNull(assemblies, nameof(assemblies));

            if (this.compositionAssemblies == null)
            {
                this.compositionAssemblies = new HashSet<Assembly>(assemblies);
            }
            else
            {
                this.compositionAssemblies.AddRange(assemblies);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the assembly containing the composition parts.
        /// </summary>
        /// <param name="assembly">The composition assembly.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided assembly is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.WithAssemblies(new[] { assembly });
        }

        /// <summary>
        /// Adds the composition parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided parts are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithParts(IEnumerable<Type> parts)
        {
            Requires.NotNull(parts, nameof(parts));

            if (this.CompositionParts == null)
            {
                this.CompositionParts = new HashSet<Type>(parts);
            }
            else
            {
                this.CompositionParts.AddRange(parts);
            }

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the composition parts.
        /// </summary>
        /// <param name="part">The composition part.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the provided part is added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithPart(Type part)
        {
            Requires.NotNull(part, nameof(part));

            return this.WithParts(new[] { part });
        }

        /// <summary>
        /// Adds the factory export.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="factory">The factory.</param>
        /// <param name="isSingleton">If set to <c>true</c>, the factory returns a shared component, otherwise an instance component.</param>
        /// <param name="allowMultiple">Indicates whether multiple registrations are allowed.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        public virtual TBuilder WithFactory<TContract>(Func<TContract> factory, bool isSingleton = false, bool allowMultiple = false)
        {
            Requires.NotNull(factory, nameof(factory));

            this.Registry.Add(new AppServiceInfo(
                                    typeof(TContract),
                                    ctx => factory(),
                                    isSingleton ? AppServiceLifetime.Singleton : AppServiceLifetime.Transient)
                                {
                                    AllowMultiple = allowMultiple,
                                });

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the registrations.
        /// </summary>
        /// <param name="registrations">A variable-length parameters list containing registrations.</param>
        /// <returns>
        /// A TBuilder.
        /// </returns>
        public virtual TBuilder WithRegistration(params IAppServiceInfo[] registrations)
        {
            Requires.NotNull(registrations, nameof(registrations));

            registrations.ForEach(r =>
                {
                    this.Registry.Add(r);
                    if (r.ContractType != null) { this.WithPart(r.ContractType); }
                    if (r.InstanceType != null) { this.WithPart(r.InstanceType); }
                });

            return (TBuilder)this;
        }

        /// <summary>
        /// Adds the export provider.
        /// </summary>
        /// <remarks>
        /// Can be used multiple times, the factories are added to the existing ones.
        /// </remarks>
        /// <param name="conventionsRegistrar">The conventions registrar.</param>
        /// <returns>
        /// This builder.
        /// </returns>
        public virtual TBuilder WithConventionsRegistrar(IConventionsRegistrar conventionsRegistrar)
        {
            Requires.NotNull(conventionsRegistrar, nameof(conventionsRegistrar));

            var registrars = this.RegistrationContext.Registrars?.ToList() ?? new List<IConventionsRegistrar>();
            registrars.Add(conventionsRegistrar);
            this.RegistrationContext.Registrars = registrars;

            return (TBuilder)this;
        }

        /// <summary>
        /// Creates the container with the provided configuration asynchronously.
        /// </summary>
        /// <returns>A new container with the provided configuration.</returns>
        public virtual ICompositionContext CreateContainer()
        {
            ICompositionContext? container = null;
            Profiler.WithInfoStopwatch(
                () =>
                {
                    var assemblies = this.GetCompositionAssemblies();

                    container = this.CreateContainerWithConventions(assemblies);
                },
                this.Logger);

            return container!;
        }

        /// <summary>
        /// Factory method for creating the conventions builder.
        /// </summary>
        /// <returns>A newly created conventions builder.</returns>
        protected abstract IConventionsBuilder CreateConventionsBuilder();

        /// <summary>
        /// Creates a new composition container based on the provided conventions and assembly parts.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <param name="parts">The parts candidating for composition.</param>
        /// <returns>
        /// A new composition container.
        /// </returns>
        protected abstract ICompositionContext CreateContainerCore(IConventionsBuilder conventions, IEnumerable<Type> parts);

        /// <summary>
        /// Gets the composition assemblies.
        /// </summary>
        /// <returns>An enumeration of assemblies used for composition.</returns>
        protected IEnumerable<Assembly> GetCompositionAssemblies()
        {
            return (IEnumerable<Assembly>)this.compositionAssemblies ?? this.GetAssemblies();
        }

        /// <summary>
        /// Gets the convention assemblies.
        /// </summary>
        /// <param name="fallbackAssemblies">The fallback assemblies, used if no convention assemblies are provided.</param>
        /// <returns>An enumeration of assemblies used for conventions.</returns>
        protected IEnumerable<Assembly> GetConventionAssemblies(IEnumerable<Assembly> fallbackAssemblies)
        {
            return this.conventionAssemblies ?? fallbackAssemblies;
        }

        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <param name="assemblies">The assemblies containing the conventions.</param>
        /// <param name="parts">The parts.</param>
        /// <returns>
        /// The convention builder.
        /// </returns>
        protected virtual IConventionsBuilder GetConventions(IEnumerable<Assembly> assemblies, IList<Type> parts)
        {
            var conventions = this.ConventionsBuilder ?? this.CreateConventionsBuilder();

            if (this.Logger.IsDebugEnabled())
            {
                var assemblyNames = assemblies.Select(a => a.GetName().Name).ToList();
                this.Logger.Debug("{operation}. Convention assemblies: {assemblies}.", nameof(this.GetConventions), assemblyNames);
            }

            Profiler.WithInfoStopwatch(
                () =>
                {
                    conventions.RegisterConventionsFrom(assemblies, parts, this.RegistrationContext);
                },
                this.Logger);

            return conventions;
        }

        /// <summary>
        /// Asserts the required service is not missing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="service">The service.</param>
        protected void AssertRequiredService<TService>(TService service)
        {
            if (service == null)
            {
                throw new InvalidOperationException(string.Format(Strings.CompositionContainerBuilderBase_RequiredServiceMissing_Exception, typeof(TService).FullName));
            }
        }

        /// <summary>
        /// Gets the composition settings.
        /// </summary>
        /// <returns>
        /// The composition settings.
        /// </returns>
        protected virtual CompositionSettings GetSettings()
        {
            return null;
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns>The assemblies.</returns>
        private IList<Assembly> GetAssemblies(string? searchPattern = null)
        {
            searchPattern ??= this.GetSettings()?.AssemblyFileNamePattern;

            this.Logger.Debug("{operation}. With assemblies matching pattern '{searchPattern}'.", nameof(this.GetAssemblies), searchPattern);

            IList<Assembly>? assemblies = null;

            Profiler.WithDebugStopwatch(
                () =>
                {
                    var appAssemblies = this.AppRuntime.GetAppAssemblies();
                    appAssemblies = this.WhereNotSystemAssemblies(appAssemblies);

                    if (string.IsNullOrWhiteSpace(searchPattern))
                    {
                        assemblies = appAssemblies.ToList();
                    }
                    else
                    {
                        var regex = new Regex(searchPattern);
                        assemblies = appAssemblies.Where(a => regex.IsMatch(a.FullName)).ToList();
                    }
                },
                this.Logger);

            return assemblies!;
        }

        /// <summary>
        /// Filters out the system assemblies from the provided assemblies.
        /// </summary>
        /// <param name="assemblies">The convention assemblies.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process where not system assemblies in this
        /// collection.
        /// </returns>
        private IEnumerable<Assembly> WhereNotSystemAssemblies(IEnumerable<Assembly> assemblies)
        {
            return assemblies.Where(a => !a.IsSystemAssembly());
        }

        /// <summary>
        /// Creates the container with the registered conventions.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The composition container.</returns>
        private ICompositionContext CreateContainerWithConventions(IEnumerable<Assembly> assemblies)
        {
            var parts = this.GetCompositionParts(assemblies);
            var conventionAssemblies = this.GetConventionAssemblies(assemblies);
            var conventions = this.GetConventions(conventionAssemblies, parts);

            var container = this.CreateContainerCore(conventions, parts);
            return container;
        }

        /// <summary>
        /// Gets the composition parts.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>The composition parts.</returns>
        private IList<Type> GetCompositionParts(IEnumerable<Assembly> assemblies)
        {
            var parts = assemblies
                .SelectMany(a => this.TypeLoader.GetExportedTypes(a))
                .Where(ConventionsBuilderExtensions.IsPartCandidate)
                .ToList();
            if (this.CompositionParts != null)
            {
                parts.AddRange(this.CompositionParts.Where(ConventionsBuilderExtensions.IsPartCandidate));
            }

            return parts;
        }

        /// <summary>
        /// An application service information serviceRegistry.
        /// </summary>
        protected class AppServiceInfoRegistry : IAppServiceInfoProvider
        {
            private IList<IAppServiceInfo> appServiceInfos = new List<IAppServiceInfo>();

            /// <summary>
            /// Adds an <see cref="IAppServiceInfo"/>.
            /// </summary>
            /// <param name="appServiceInfo">The Application service Information to add.</param>
            public void Add(IAppServiceInfo appServiceInfo)
            {
                this.appServiceInfos.Add(appServiceInfo);
            }

            /// <summary>
            /// Gets an enumeration of application service information objects.
            /// </summary>
            /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
            /// <param name="registrationContext">Context for the registration.</param>
            /// <returns>
            /// An enumeration of application service information objects and their associated contract type.
            /// </returns>
            public IEnumerable<(Type contractType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
            {
                return this.appServiceInfos.Select(i => (i.ContractType, i));
            }
        }
    }
}