﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceCollectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the logging service collection extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Configuration
{
    using Kephas;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Extensions.Configuration;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Configuration extensions.
    /// </summary>
    public static class ConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the options extensions.
        /// </summary>
        /// <param name="ambientServices">The ambient services to act on.</param>
        /// <returns>
        /// The provided ambient services.
        /// </returns>
        public static IAmbientServices ConfigureOptionsExtensions(this IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            var serviceCollection = AmbientServicesExtensions.GetService<IServiceCollection>(ambientServices);

            serviceCollection.Replace(ServiceDescriptor.Scoped(typeof(Microsoft.Extensions.Options.IOptionsSnapshot<>), typeof(OptionsSnapshot<>)));

            return ambientServices;
        }
    }
}