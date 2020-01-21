﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPluginManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the nu get plugin manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::NuGet.Configuration;
    using global::NuGet.Frameworks;
    using global::NuGet.Packaging;
    using global::NuGet.Packaging.Core;
    using global::NuGet.Protocol.Core.Types;
    using global::NuGet.Resolver;
    using global::NuGet.Versioning;
    using Kephas;
    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Manager for plugins based on the NuGet infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class NuGetPluginManager : Loggable, IPluginManager
    {
        /// <summary>
        /// The default packages folder.
        /// </summary>
        public const string DefaultPackagesFolder = ".packages";

        // check the following resource for documentation
        // https://martinbjorkstrom.com/posts/2018-09-19-revisiting-nuget-client-libraries

        private ISettings settings;
        private PluginsSettings pluginsSettings;
        private CoreSettings coreSettings;
        private global::NuGet.Common.ILogger nativeLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPluginManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="coreConfig">The core configuration.</param>
        /// <param name="pluginsConfig">The plugins configuration.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public NuGetPluginManager(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IConfiguration<CoreSettings> coreConfig,
            IConfiguration<PluginsSettings> pluginsConfig,
            ILogManager logManager = null)
            : base(logManager)
        {
            this.AppRuntime = appRuntime;
            this.ContextFactory = contextFactory;
            this.nativeLogger = new NuGetLogger(this.Logger);
            this.pluginsSettings = pluginsConfig.Settings;
            this.coreSettings = coreConfig.Settings;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        /// <value>
        /// The application runtime.
        /// </value>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the context factory.
        /// </summary>
        /// <value>
        /// The context factory.
        /// </value>
        protected IContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public virtual async Task<IEnumerable<IPluginInfo>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
        {
            var searchContext = this.CreateSearchContext(filter);
            var repositories = this.GetSourceRepositories();

            cancellationToken.ThrowIfCancellationRequested();

            using (var cacheContext = new SourceCacheContext())
            {
                var availablePackages = new HashSet<IPackageSearchMetadata>();
                foreach (var sourceRepository in repositories)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>().PreserveThreadContext();
                    var searchFilter = new SearchFilter(includePrerelease: searchContext.IncludePrerelease);
                    searchFilter.OrderBy = SearchOrderBy.Id;
                    searchFilter.IncludeDelisted = false;

                    try
                    {
                        var packages = await searchResource.SearchAsync(
                            searchContext.SearchTerm ?? this.pluginsSettings.SearchTerm ?? "plugin",
                            searchFilter,
                            searchContext.Skip,
                            searchContext.Take,
                            this.nativeLogger,
                            cancellationToken).PreserveThreadContext();

                        availablePackages.AddRange(packages);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, "Could not access source repository '{repository}'.", sourceRepository.PackageSource.Source);
                    }
                }

                return availablePackages.Select(this.ToPluginInfo);
            }
        }

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// An enumeration of installed plugins.
        /// </returns>
        public virtual IEnumerable<IPlugin> GetInstalledPlugins()
        {
            var pluginsFolder = this.AppRuntime.GetPluginsFolder();
            return Directory.EnumerateDirectories(pluginsFolder)
                    .Select(d => new Plugin(new PluginInfo(Path.GetFileName(d), PluginHelper.GetPluginVersion(d))) { FolderPath = d });
        }

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the install operation result.
        /// </returns>
        public virtual async Task<IOperationResult> InstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            var (_, state, version) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.None)
            {
                throw new InvalidOperationException($"Plugin {plugin} is already installed. State: '{state}', version: '{version}'.");
            }

            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;

            var repositories = this.GetSourceRepositories();
            using (var cacheContext = new SourceCacheContext())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packagesFolder = this.GetPackagesFolder();

                var pluginPackageIdentity = this.ToPackageIdentity(plugin);
                var downloadResult = await this.DownloadPackageAsync(pluginPackageIdentity, packagesFolder, repositories, cacheContext, cancellationToken).PreserveThreadContext();
                if (downloadResult.Status != DownloadResourceResultStatus.Available)
                {
                    throw new InvalidOperationException($"Plugin package {pluginPackageIdentity} not available ({downloadResult.Status}).");
                }

                var currentFramework = this.AppRuntime.GetAppFramework();
                var nugetFramework = NuGetFramework.ParseFolder(currentFramework);
                var dependencies = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                await this.GetPackageDependenciesAsync(
                    pluginPackageIdentity,
                    nugetFramework,
                    cacheContext,
                    this.nativeLogger,
                    repositories,
                    dependencies).PreserveThreadContext();

                cancellationToken.ThrowIfCancellationRequested();

                var resolverContext = new PackageResolverContext(
                    this.pluginsSettings.ResolverDependencyBehavior,
                    new[] { plugin.Id },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    dependencies,
                    repositories.Select(s => s.PackageSource),
                    this.nativeLogger);

                var resolver = new PackageResolver();
                var dependenciesToInstall = resolver.Resolve(resolverContext, cancellationToken)
                    .Select(p => dependencies.Single(x => PackageIdentityComparer.Default.Equals(x, p)));

                cancellationToken.ThrowIfCancellationRequested();

                var packageReaders = await this.GetPackageReadersAsync(
                    repositories,
                    cacheContext,
                    packagesFolder,
                    dependenciesToInstall,
                    cancellationToken).PreserveThreadContext();

                // get the right casing of the package ID, the provided casing might not be the right one.
                pluginPackageIdentity = dependenciesToInstall.FirstOrDefault(d => d.Equals(pluginPackageIdentity)) ?? pluginPackageIdentity;

                pluginInfo = new PluginInfo(pluginPackageIdentity.Id, pluginPackageIdentity.Version.ToString());

                var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsFolder(), pluginPackageIdentity.Id);
                if (!Directory.Exists(pluginFolder))
                {
                    Directory.CreateDirectory(pluginFolder);
                }

                var frameworkReducer = new FrameworkReducer();
                foreach (var packageReader in packageReaders)
                {
                    await this.InstallPluginBinAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                    await this.InstallPluginContentAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                }

                PluginHelper.SetPluginData(pluginFolder, PluginState.PendingInitialization, pluginPackageIdentity.Version.ToString());

                pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };
            }

            this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization.", plugin);

            var result = new OperationResult().MergeMessage($"Plugin {plugin} successfully installed, awaiting initialization.");
            result["Plugin"] = pluginData;

            return result;
        }

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall operation result.
        /// </returns>
        public virtual async Task<IOperationResult> UninstallPluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();
            var (pluginFolder, state, version) = this.GetInstalledPluginData(plugin);
            if (state == PluginState.None)
            {
                throw new InvalidOperationException($"Plugin {plugin} is already uninstalled.");
            }

            if (state == PluginState.Enabled || state == PluginState.Disabled)
            {
                var result = await this.UninitializePluginAsync(plugin, context, cancellationToken).PreserveThreadContext();
                if (result.HasErrors())
                {
                    return result;
                }
            }

            Directory.Delete(pluginFolder, recursive: true);

            this.Logger.Info("Plugin {plugin} successfully uninstalled.", plugin);

            return new OperationResult().MergeMessage($"Plugin {plugin} successfully uninstalled.");
        }

        /// <summary>
        /// Initializes the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the initialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult> InitializePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            var (pluginFolder, state, version) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.PendingInitialization)
            {
                throw new InvalidOperationException($"Cannot initialize plugin {plugin} while in state '{state}'.");
            }

            await this.InitializePluginDataAsync(plugin, context, cancellationToken).PreserveThreadContext();

            PluginHelper.SetPluginData(pluginFolder, PluginState.Enabled, version);

            this.Logger.Info("Plugin {plugin} successfully initialized.", plugin);

            return new OperationResult().MergeMessage($"Plugin {plugin} successfully initialized.");
        }

        /// <summary>
        /// Uninitializes the plugin asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninitialize operation result.
        /// </returns>
        public virtual async Task<IOperationResult> UninitializePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            var (pluginFolder, state, version) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.Enabled && state != PluginState.Disabled)
            {
                throw new InvalidOperationException($"Cannot uninitialize plugin {plugin} while in state '{state}'.");
            }

            await this.UninitializePluginDataAsync(plugin, context, cancellationToken).PreserveThreadContext();

            PluginHelper.SetPluginData(pluginFolder, PluginState.PendingInitialization, version);

            this.Logger.Info("Plugin {plugin} successfully uninitialized.", plugin);

            return new OperationResult().MergeMessage($"Plugin {plugin} successfully uninitialized.");
        }

        /// <summary>
        /// Enables the plugin asynchronously if the plugin was previously disabled.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult> EnablePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            var (pluginFolder, state, version) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.Disabled)
            {
                throw new InvalidOperationException($"Cannot enable plugin {plugin} while in state '{state}'.");
            }

            PluginHelper.SetPluginData(pluginFolder, PluginState.Enabled, version);

            this.Logger.Info("Plugin {plugin} successfully enabled.", plugin);

            return new OperationResult().MergeMessage($"Plugin {plugin} successfully enabled.");
        }

        /// <summary>
        /// Disables the plugin asynchronously if the plugin was previously enabled.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        public virtual async Task<IOperationResult> DisablePluginAsync(PluginIdentity plugin, IContext context = null, CancellationToken cancellationToken = default)
        {
            this.AssertPluginsDisabled();

            var (pluginFolder, state, version) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.Enabled)
            {
                throw new InvalidOperationException($"Cannot disable plugin {plugin} while in state '{state}'.");
            }

            PluginHelper.SetPluginData(pluginFolder, PluginState.Disabled, version);

            this.Logger.Warn("Plugin {plugin} successfully disabled.", plugin);

            return new OperationResult().MergeMessage($"Plugin {plugin} successfully disabled.");
        }

        /// <summary>
        /// Asserts that the plugins are disabled.
        /// </summary>
        protected virtual void AssertPluginsDisabled()
        {
            if (this.AppRuntime.PluginsEnabled())
            {
                throw new InvalidOperationException("Cannot proceed with the operation while the plugins are enabled. Please start the application in setup mode to disable them and then rerun the operation.");
            }
        }

        /// <summary>
        /// Gets the installed plugin data.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <returns>
        /// The installed plugin data.
        /// </returns>
        protected virtual (string pluginFolder, PluginState state, string version) GetInstalledPluginData(PluginIdentity plugin)
        {
            var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsFolder(), plugin.Id);
            var (state, version) = PluginHelper.GetPluginData(pluginFolder);
            return (pluginFolder, state, version);
        }

        /// <summary>
        /// Initializes the plugin data asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task InitializePluginDataAsync(PluginIdentity plugin, IContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Uninitializes the plugin data asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual Task UninitializePluginDataAsync(PluginIdentity plugin, IContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the packages folder.
        /// </summary>
        /// <param name="defaultPackagesFolder">Optional. The default packages folder.</param>
        /// <returns>
        /// The packages folder.
        /// </returns>
        protected virtual string GetPackagesFolder(string defaultPackagesFolder = null)
        {
            defaultPackagesFolder = defaultPackagesFolder ?? DefaultPackagesFolder;
            if (string.IsNullOrEmpty(this.pluginsSettings.PackagesFolder))
            {
                var settings = this.GetSettings();
                var repositoryPathSettings = settings.GetSection("config")?.GetFirstItemWithAttribute<AddItem>("key", "repositoryPath");
                var repositoryPath = repositoryPathSettings?.Value;
                var fullRepositoryPath = string.IsNullOrEmpty(repositoryPath)
                    ? this.AppRuntime.GetFullPath(defaultPackagesFolder)
                    : Path.IsPathRooted(repositoryPath)
                        ? repositoryPath
                        : Path.GetFullPath(Path.Combine(this.GetSettingsFolderPath(), repositoryPath));
                return fullRepositoryPath;
            }

            return Path.IsPathRooted(this.pluginsSettings.PackagesFolder)
                ? this.pluginsSettings.PackagesFolder
                : Path.Combine(this.AppRuntime.GetAppLocation(), this.pluginsSettings.PackagesFolder);
        }

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        /// <returns>
        /// The settings folder path.
        /// </returns>
        protected virtual string GetSettingsFolderPath()
        {
            return string.IsNullOrEmpty(this.pluginsSettings.NuGetConfigPath)
                ? this.AppRuntime.GetAppConfigFullPath()
                : this.AppRuntime.GetFullPath(this.pluginsSettings.NuGetConfigPath);
        }

        /// <summary>
        /// Gets the NuGet settings.
        /// </summary>
        /// <returns>
        /// The NuGet settings.
        /// </returns>
        protected virtual ISettings GetSettings()
        {
            if (this.settings == null)
            {
                string root = this.GetSettingsFolderPath();
                this.settings = new Settings(root);
            }

            return this.settings;
        }

        /// <summary>
        /// Gets the source repository provider.
        /// </summary>
        /// <returns>
        /// The source repository provider.
        /// </returns>
        protected virtual SourceRepositoryProvider GetSourceRepositoryProvider()
        {
            var settings = this.GetSettings();
            var sourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(sourceProvider, Repository.Provider.GetCoreV3());
            return sourceRepositoryProvider;
        }

        /// <summary>
        /// Gets the source repositories.
        /// </summary>
        /// <returns>
        /// The source repositories.
        /// </returns>
        protected virtual IList<SourceRepository> GetSourceRepositories()
        {
            var sourceRepositoryProvider = this.GetSourceRepositoryProvider();
            var repositories = sourceRepositoryProvider.GetRepositories();
            // TODO test whether repository is available
            return repositories.ToList();
        }

        /// <summary>
        /// Converts a searchMetadata to a plugin information.
        /// </summary>
        /// <param name="searchMetadata">The search metadata.</param>
        /// <returns>
        /// SearchMetadata as an IPluginInfo.
        /// </returns>
        protected virtual IPluginInfo ToPluginInfo(IPackageSearchMetadata searchMetadata)
        {
            return new PluginInfo(
                searchMetadata.Identity.Id,
                searchMetadata.Identity.Version.ToString(),
                searchMetadata.Description,
                searchMetadata.Tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Converts a plugin identity to a package identity.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// PluginIdentity as a PackageIdentity.
        /// </returns>
        protected virtual PackageIdentity ToPackageIdentity(PluginIdentity pluginIdentity)
        {
            return new PackageIdentity(pluginIdentity.Id, NuGetVersion.Parse(pluginIdentity.Version));
        }

        /// <summary>
        /// Creates the search context.
        /// </summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <returns>
        /// The new search context.
        /// </returns>
        protected virtual ISearchContext CreateSearchContext(Action<ISearchContext> filter)
        {
            return this.ContextFactory.CreateContext<SearchContext>().Merge(filter);
        }

        /// <summary>
        /// Gets the package path resolver.
        /// </summary>
        /// <returns>
        /// The package path resolver.
        /// </returns>
        protected virtual PackagePathResolver GetPackagePathResolver()
        {
            return new PackagePathResolver(this.GetPackagesFolder());
        }

        /// <summary>
        /// Installs the plugin library items asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task InstallPluginBinAsync(PluginIdentity plugin, IContext context, NuGetFramework nugetFramework, string pluginFolder, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string libFolderName = "lib";

            var libItems = packageReader.GetLibItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, libItems.Select(x => x.TargetFramework));

            var libItem = libItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(libItem?.HasEmptyFolder ?? true))
            {
                await packageReader.CopyFilesAsync(pluginFolder, libItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, libFolderName, flatten: true), this.nativeLogger, cancellationToken).PreserveThreadContext();
            }

            var libFolder = Path.Combine(pluginFolder, libFolderName);
            if (Directory.Exists(libFolder))
            {
                Directory.Delete(libFolder, recursive: true);
            }
        }

        /// <summary>
        /// Installs the plugin content items asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task InstallPluginContentAsync(PluginIdentity plugin, IContext context, NuGetFramework nugetFramework, string pluginFolder, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string contentFolderName = "content";

            var contentItems = packageReader.GetContentItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, contentItems.Select(x => x.TargetFramework));

            var contentItem = contentItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(contentItem?.HasEmptyFolder ?? true))
            {
                await packageReader.CopyFilesAsync(pluginFolder, contentItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, contentFolderName, flatten: false), this.nativeLogger, cancellationToken).PreserveThreadContext();
            }

            var contentFolder = Path.Combine(pluginFolder, contentFolderName);
            if (Directory.Exists(contentFolder))
            {
                Directory.Delete(contentFolder, recursive: true);
            }
        }

        private string ExtractPackageFile(string sourceFile, string targetPath, string subFolder, bool flatten)
        {
            targetPath = targetPath.Replace('/', Path.DirectorySeparatorChar);
            var fileName = Path.GetFileName(targetPath);
            var searchTerm = Path.DirectorySeparatorChar + subFolder + Path.DirectorySeparatorChar;
            var indexSearchTerm = targetPath.IndexOf(searchTerm);
            if (indexSearchTerm >= 0)
            {
                targetPath = flatten
                    ? targetPath.Substring(0, indexSearchTerm + 1) + fileName
                    : targetPath.Substring(0, indexSearchTerm + 1) + targetPath.Substring(indexSearchTerm + searchTerm.Length);
            }

            var directory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(sourceFile, targetPath, overwrite: true);
            return targetPath;
        }

        private async Task<List<PackageReaderBase>> GetPackageReadersAsync(IList<SourceRepository> repositories, SourceCacheContext cacheContext, string packagesFolder, IEnumerable<SourcePackageDependencyInfo> dependenciesToInstall, CancellationToken cancellationToken)
        {
            var downloadContext = new PackageDownloadContext(cacheContext);
            var downloadResources = await this.GetDownloadResourcesAsync(repositories, cancellationToken).PreserveThreadContext();

            var packageReaders = new List<PackageReaderBase>();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var dependency in dependenciesToInstall)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packagePathResolver = this.GetPackagePathResolver();
                var installedPath = packagePathResolver.GetInstalledPath(dependency);
                if (installedPath != null)
                {
                    packageReaders.Add(new PackageFolderReader(installedPath));
                }
                else
                {
                    var downloadResult = await this.DownloadPackageAsync(
                            dependency,
                            packagesFolder,
                            downloadResources,
                            downloadContext,
                            cancellationToken).PreserveThreadContext();
                    packageReaders.Add(downloadResult.PackageReader);
                }
            }

            return packageReaders;
        }

        private async Task<List<DownloadResource>> GetDownloadResourcesAsync(IList<SourceRepository> repositories, CancellationToken cancellationToken = default)
        {
            var downloadResources = new List<DownloadResource>();
            foreach (var sourceRepository in repositories)
            {
                var downloadResource = await sourceRepository.GetResourceAsync<DownloadResource>(cancellationToken).PreserveThreadContext();
                downloadResources.Add(downloadResource);
            }

            return downloadResources;
        }

        private async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageId, string packagesFolder, IList<SourceRepository> repositories, SourceCacheContext cacheContext, CancellationToken cancellationToken)
        {
            var downloadContext = new PackageDownloadContext(cacheContext);
            var downloadResources = await this.GetDownloadResourcesAsync(repositories, cancellationToken).PreserveThreadContext();
            return await this.DownloadPackageAsync(packageId, packagesFolder, downloadResources, downloadContext, cancellationToken).PreserveThreadContext();
        }

        private async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageId, string packagesFolder, IEnumerable<DownloadResource> downloadResources, PackageDownloadContext downloadContext, CancellationToken cancellationToken)
        {
            DownloadResourceResult downloadResult = null;
            if (packageId is SourcePackageDependencyInfo dependencyInfo)
            {
                var downloadResource = await dependencyInfo.Source.GetResourceAsync<DownloadResource>(cancellationToken).PreserveThreadContext();
                downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                    packageId,
                    downloadContext,
                    packagesFolder,
                    this.nativeLogger,
                    default).PreserveThreadContext();
            }
            else
            {
                foreach (var downloadResource in downloadResources)
                {
                    downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                        packageId,
                        downloadContext,
                        packagesFolder,
                        this.nativeLogger,
                        default).PreserveThreadContext();

                    if (downloadResult.Status == DownloadResourceResultStatus.Available)
                    {
                        break;
                    }
                }
            }

            if (downloadResult?.Status != DownloadResourceResultStatus.Available)
            {
                throw new InvalidOperationException($"Package {packageId} not available ({downloadResult?.Status}).");
            }

            return downloadResult;
        }

        private async Task GetPackageDependenciesAsync(
            PackageIdentity package,
            NuGetFramework framework,
            SourceCacheContext cacheContext,
            global::NuGet.Common.ILogger logger,
            IList<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package))
            {
                return;
            }

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>().PreserveThreadContext();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None).PreserveThreadContext();

                if (dependencyInfo == null)
                {
                    continue;
                }

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await this.GetPackageDependenciesAsync(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework,
                        cacheContext,
                        logger,
                        repositories,
                        availablePackages);
                }
            }
        }
    }
}