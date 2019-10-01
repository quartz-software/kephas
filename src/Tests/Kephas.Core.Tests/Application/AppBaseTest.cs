﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppBaseTest
    {
        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_static_instance_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();

            IAmbientServices ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            Assert.IsNotNull(ambientServices);
            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task ConfigureAmbientServicesAsync_ambient_services_explicit_instance_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();

            IAmbientServices ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            Assert.AreSame(app.AmbientServices, ambientServices);
            Assert.AreSame(app.AmbientServices, appContext.AmbientServices);
        }

        [Test]
        public async Task BootstrapAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            IAmbientServices ambientServices = null;
            var app = new TestApp(async b => ambientServices = b.WithCompositionContainer(compositionContext));
            await app.BootstrapAsync();

            appManager.Received(1).InitializeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task BootstrapAsync_shutdown_instruction_stops_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppShutdownAwaiter>();
            termAwaiter.WaitForShutdownSignalAsync(Arg.Any<CancellationToken>())
                .Returns((12, AppShutdownInstruction.Shutdown));

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>())
                .Returns(appManager);
            compositionContext.GetExport<IAppShutdownAwaiter>(Arg.Any<string>())
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            Assert.AreEqual(12, appContext.AppResult);
        }

        [Test]
        public async Task BootstrapAsync_none_instruction_does_not_stop_application()
        {
            var appManager = Substitute.For<IAppManager>();
            var termAwaiter = Substitute.For<IAppShutdownAwaiter>();
            termAwaiter.WaitForShutdownSignalAsync(Arg.Any<CancellationToken>())
                .Returns((23, AppShutdownInstruction.Ignore));

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>())
                .Returns(appManager);
            compositionContext.GetExport<IAppShutdownAwaiter>(Arg.Any<string>())
                .Returns(termAwaiter);

            var app = new TestApp(async b => b.WithCompositionContainer(compositionContext));
            var appContext = await app.BootstrapAsync();

            appManager.Received(0).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
            Assert.AreEqual(23, appContext.AppResult);
        }

        [Test]
        public async Task ShutdownAsync_appManager_invoked()
        {
            var appManager = Substitute.For<IAppManager>();

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport<IAppManager>(Arg.Any<string>()).Returns(appManager);

            var ambientServices = new AmbientServices()
                .WithCompositionContainer(compositionContext);
            var app = new TestApp(ambientServices: ambientServices);
            await app.ShutdownAsync();

            appManager.Received(1).FinalizeAppAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>());
        }
    }

    public class TestApp : AppBase
    {
        private readonly Func<IAmbientServices, Task> asyncConfig;

        public TestApp(Func<IAmbientServices, Task> asyncConfig = null, IAmbientServices ambientServices = null)
            : base(ambientServices ?? new AmbientServices())
        {
            this.asyncConfig = asyncConfig;
        }

        /// <summary>
        /// Configures the ambient services asynchronously.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        protected override async void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            if (this.asyncConfig != null)
            {
                await this.asyncConfig(ambientServices);
            }
        }
    }
}