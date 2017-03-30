﻿namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services.Transitioning;

    using NSubstitute;

    using NUnit.Framework;

    using TaskHelper = Kephas.Threading.Tasks.TaskHelper;

    [TestFixture]
    public class AppInitializerBaseTest
    {
        [Test]
        public void InitializeAsync_exception()
        {
            var appInitializer = new TestAppInitializer(new AmbiguousMatchException());

            Assert.That(() => appInitializer.InitializeAsync(Substitute.For<IAppContext>()), Throws.TypeOf<AmbiguousMatchException>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsFaulted);
        }

        [Test]
        public async Task InitializeAsync_success()
        {
            var appInitializer = new TestAppInitializer();

            await appInitializer.InitializeAsync(Substitute.For<IAppContext>());

            Assert.IsTrue(appInitializer.GetInitializationMonitor().IsCompletedSuccessfully);
        }

        private class TestAppInitializer : AppInitializerBase
        {
            private readonly Exception exception;

            public TestAppInitializer()
            {
            }

            public TestAppInitializer(Exception exception)
            {
                this.exception = exception;
            }

            public InitializationMonitor<IAppInitializer> GetInitializationMonitor()
            {
                return this.InitializationMonitor;
            }

            protected override Task InitializeCoreAsync(IAppContext appContext, CancellationToken cancellationToken)
            {
                if (this.exception != null)
                {
                    throw this.exception;
                }

                return TaskHelper.CompletedTask;
            }
        }
    }
}