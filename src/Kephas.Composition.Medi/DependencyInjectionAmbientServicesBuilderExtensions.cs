﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionAmbientServicesBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dependency injection ambient services builder extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Composition.Hosting;
    using Kephas.Composition.Medi.Hosting;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Microsoft.Extensions.DependencyInjection related ambient services builder extensions.
    /// </summary>
    public static class DependencyInjectionAmbientServicesBuilderExtensions
    {
        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <returns>The provided ambient services builder.</returns>
        public static AmbientServicesBuilder WithDependencyInjectionCompositionContainer(this AmbientServicesBuilder ambientServicesBuilder, Action<DependencyInjectionCompositionContainerBuilder> containerBuilderConfig = null)
        {
            Requires.NotNull(ambientServicesBuilder, nameof(ambientServicesBuilder));

            var containerBuilder = new DependencyInjectionCompositionContainerBuilder(new CompositionRegistrationContext(ambientServicesBuilder.AmbientServices));

            containerBuilderConfig?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            return ambientServicesBuilder.WithCompositionContainer(container);
        }
    }
}