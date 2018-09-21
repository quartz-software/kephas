﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpace.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Container class for data contexts indexed by contained entity types.
    /// </summary>
    public class DataSpace : Context, IDataSpace
    {
        /// <summary>
        /// The data context factory.
        /// </summary>
        private readonly IDataContextFactory dataContextFactory;

        /// <summary>
        /// The data store selector.
        /// </summary>
        private readonly IDataStoreSelector dataStoreSelector;

        /// <summary>
        /// Context for the operation.
        /// </summary>
        private IContext operationContext;

        /// <summary>
        /// The data contexts' map.
        /// </summary>
        private IDictionary<string, IDataContext> dataContextMap = new Dictionary<string, IDataContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="dataStoreSelector">The data store selector.</param>
        public DataSpace(
            ICompositionContext compositionContext,
            IDataContextFactory dataContextFactory,
            IDataStoreSelector dataStoreSelector)
            : this(compositionContext, dataContextFactory, dataStoreSelector, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSpace"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="dataContextFactory">The data context factory.</param>
        /// <param name="dataStoreSelector">The data store selector.</param>
        /// <param name="isThreadSafe">True if this object is thread safe.</param>
        protected DataSpace(
            ICompositionContext compositionContext,
            IDataContextFactory dataContextFactory,
            IDataStoreSelector dataStoreSelector,
            bool isThreadSafe)
            : base(compositionContext, isThreadSafe)
        {
            Requires.NotNull(dataContextFactory, nameof(dataContextFactory));
            Requires.NotNull(dataStoreSelector, nameof(dataStoreSelector));

            this.dataContextFactory = dataContextFactory;
            this.dataStoreSelector = dataStoreSelector;
        }

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count => this.dataContextMap.Count;

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        public virtual IDataContext this[Type entityType, IContext context = null]
        {
            get
            {
                var dataStoreName = this.dataStoreSelector.GetDataStoreName(entityType, this.operationContext);
                var dataContext = this.dataContextMap.TryGetValue(dataStoreName);
                if (dataContext == null)
                {
                    dataContext = this.dataContextFactory.CreateDataContext(dataStoreName, this.operationContext);
                    this.dataContextMap.Add(dataStoreName, dataContext);
                }

                return dataContext;
            }
        }

        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        public virtual IDataContext this[ITypeInfo entityType, IContext context = null] => this[entityType.AsType()];

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            this.ClearDataContextMap();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IDataContext> GetEnumerator()
        {
            return this.dataContextMap.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        public virtual void Initialize(IContext context = null)
        {
            this.operationContext = context;
            if (this.Identity == null)
            {
                this.Identity = context?.Identity;
            }

            this.ClearDataContextMap();
            var entityInfos = context?.InitialData();
            if (entityInfos != null)
            {
                this.dataContextMap = entityInfos
                                          .GroupBy(e => this.dataStoreSelector.GetDataStoreName(e.Entity.GetType(), this.operationContext), e => e)
                                          .ToDictionary(
                                              g => g.Key,
                                              g =>
                                                  {
                                                      var initializationContext = new Context(this.CompositionContext)
                                                                                      {
                                                                                          Identity = this.Identity,
                                                                                      }.WithInitialData(g);
                                                      return this.dataContextFactory.CreateDataContext(
                                                          g.Key,
                                                          initializationContext);
                                                  });
            }
        }

        /// <summary>
        /// Clears the data context map.
        /// </summary>
        protected virtual void ClearDataContextMap()
        {
            foreach (var dataContext in this.dataContextMap.Values)
            {
                dataContext.Dispose();
            }

            this.dataContextMap.Clear();
        }
    }
}