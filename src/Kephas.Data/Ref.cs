﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ref.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the reference base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Implementation of an entity reference.
    /// </summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    public class Ref<T> : IRef<T>
        where T : class
    {
        /// <summary>
        /// The entity information provider.
        /// </summary>
        private readonly WeakReference<IEntityInfoAware> entityRef;

        /// <summary>
        /// Name of the reference identifier.
        /// </summary>
        private readonly string refIdName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ref{T}"/> class.
        /// </summary>
        /// <param name="entityInfoAware">The entity information provider.</param>
        /// <param name="refIdName">Name of the reference identifier.</param>
        public Ref(IEntityInfoAware entityInfoAware, string refIdName)
        {
            Requires.NotNull(entityInfoAware, nameof(entityInfoAware));
            Requires.NotNullOrEmpty(refIdName, nameof(refIdName));

            this.entityRef = new WeakReference<IEntityInfoAware>(entityInfoAware);
            this.refIdName = refIdName;
            this.EntityType = typeof(T);
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        object IIdentifiable.Id => this.Id;

        /// <summary>
        /// Gets the type of the referenced entity.
        /// </summary>
        /// <value>
        /// The type of the referenced entity.
        /// </value>
        public Type EntityType { get; }

        /// <summary>
        /// Gets or sets the identifier of the referenced entity.
        /// </summary>
        /// <value>
        /// The identifier of the referenced entity.
        /// </value>
        public virtual object Id
        {
            get => this.GetEntityPropertyValue(this.refIdName);
            set => this.SetEntityPropertyValue(this.refIdName, value);
        }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        public virtual async Task<T> GetAsync(bool throwIfNotFound = true, CancellationToken cancellationToken = default)
        {
            if (Data.Id.IsEmpty(this.Id))
            {
                return null;
            }

            var entityInfo = this.GetEntityInfo();
            var dataContext = this.GetDataContext(entityInfo);

            var findContext = new FindContext<T>(dataContext, this.Id, throwIfNotFound);
            var refEntity = await dataContext.FindAsync<T>(findContext, cancellationToken).PreserveThreadContext();
            return refEntity;
        }

        /// <summary>
        /// Gets the referenced entity asynchronously.
        /// </summary>
        /// <param name="throwIfNotFound">If true and the referenced entity is not found, an exception occurs.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A task promising the referenced entity.
        /// </returns>
        async Task<object> IRef.GetAsync(bool throwIfNotFound, CancellationToken cancellationToken)
        {
            return await this.GetAsync(throwIfNotFound, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Gets the value of the indicated entity property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the entity property.
        /// </returns>
        protected virtual object GetEntityPropertyValue(string propertyName)
        {
            var entity = this.GetEntityInfo().Entity;
            return entity is IIndexable expandoEntity
                       ? expandoEntity[propertyName]
                       : entity.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Sets the value of the indicated entity property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetEntityPropertyValue(string propertyName, object value)
        {
            var entity = this.GetEntityInfo().Entity;
            if (entity is IIndexable expandoEntity)
            {
                expandoEntity[propertyName] = value;
            }
            else
            {
                entity.SetPropertyValue(propertyName, value);
            }
        }

        /// <summary>
        /// Gets entity information.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the entity has been disposed.</exception>
        /// <returns>
        /// The entity information.
        /// </returns>
        protected virtual IEntityInfo GetEntityInfo()
        {
            if (!this.entityRef.TryGetTarget(out var entity))
            {
                throw new ObjectDisposedException(this.GetType().Name, Strings.Ref_GetEntityInfo_Disposed_Exception);
            }

            return entity.GetEntityInfo();
        }

        /// <summary>
        /// Gets the data context for entity retrieval.
        /// </summary>
        /// <param name="entityInfo">Information describing the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        protected virtual IDataContext GetDataContext(IEntityInfo entityInfo)
        {
            var dataContext = entityInfo?.DataContext;
            if (dataContext == null)
            {
                throw new ArgumentNullException($"{nameof(entityInfo)}.{nameof(entityInfo.DataContext)}", Strings.Ref_GetDataContext_NullDataContext_Exception);
            }

            return dataContext;
        }
    }
}