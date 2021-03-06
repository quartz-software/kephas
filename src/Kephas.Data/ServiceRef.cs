﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRef.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service reference class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A service reference.
    /// </summary>
    /// <typeparam name="TService">Type of the referenced service.</typeparam>
    public class ServiceRef<TService> : RefBase, IServiceRef<TService>
        where TService : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRef{TService}"/> class.
        /// </summary>
        /// <param name="containerEntity">The entity containing the reference.</param>
        /// <param name="refFieldName">Name of the reference service property.</param>
        public ServiceRef(object containerEntity, string refFieldName)
            : base(containerEntity, refFieldName)
        {
            this.ServiceType = typeof(TService);
        }

        /// <summary>
        /// Gets or sets the name of the referenced service.
        /// </summary>
        /// <value>
        /// The name of the referenced service.
        /// </value>
        public virtual string? ServiceName
        {
            get => (string?)this.GetEntityPropertyValue(this.RefFieldName);
            set => this.SetEntityPropertyValue(this.RefFieldName, value);
        }

        /// <summary>
        /// Gets the type of the referenced service.
        /// </summary>
        /// <value>
        /// The type of the referenced service.
        /// </value>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the referenced service.
        /// </summary>
        /// <returns>
        /// The referenced service or <c>null</c>.
        /// </returns>
        public virtual TService? GetService()
        {
            var serviceName = this.ServiceName;
            if (string.IsNullOrEmpty(serviceName))
            {
                return default;
            }

            var namedServiceProvider = this.GetNamedServiceProvider();
            return namedServiceProvider.GetNamedService<TService>(serviceName!);
        }

        /// <summary>
        /// Gets the referenced service.
        /// </summary>
        /// <returns>
        /// The referenced service or <c>null</c>.
        /// </returns>
        object? IServiceRef.GetService() => this.GetService();

        /// <summary>
        /// Gets the named service provider.
        /// </summary>
        /// <returns>
        /// The named service provider.
        /// </returns>
        protected virtual INamedServiceProvider GetNamedServiceProvider()
        {
            var dataContext = this.GetDataContext(this.GetContainerEntityEntry());
            var compositionContext = dataContext.CompositionContext;
            return compositionContext.GetExport<INamedServiceProvider>();
        }
    }
}