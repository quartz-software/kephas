﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application context test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System.ComponentModel;

    using Kephas.Application;
    using Kephas.Composition;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppContextTest
    {
        [Test]
        public void Constructor_default_AppManifest_is_from_di_container()
        {
            var ambientServices = Substitute.For<IAmbientServices>();
            var container = Substitute.For<ICompositionContext>();
            var appManifest = Substitute.For<IAppManifest>();

            ambientServices.CompositionContainer.Returns(container);
            container.GetExport<IAppManifest>().Returns(appManifest);
            var appContext = new AppContext(ambientServices);
            Assert.AreSame(appManifest, appContext.AppManifest);
        }
    }
}