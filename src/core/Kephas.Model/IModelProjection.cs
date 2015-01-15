﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelProjection.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for model projections.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract for model projections.
    /// </summary>
    [ContractClass(typeof(ModelProjectionContractClass))]
    public interface IModelProjection : INamedElement
    {
        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        bool IsAggregated { get; }

        /// <summary>
        /// Gets the dimension elements making up this projection.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        IModelDimensionElement[] DimensionElements { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IModelProjection"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelProjection))]
    internal abstract class ModelProjectionContractClass : IModelProjection
    {
        /// <summary>
        /// Gets a value indicating whether this projection is the result of aggregating one or more projections.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is aggregated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAggregated { get; private set; }

        /// <summary>
        /// Gets the dimension elements making up this projection.
        /// </summary>
        /// <value>
        /// The dimension elements.
        /// </value>
        public IModelDimensionElement[] DimensionElements
        {
            get
            {
                Contract.Ensures(Contract.Result<IModelDimensionElement[]>() != null);
                return Contract.Result<IModelDimensionElement[]>();
            }
        }

        /// <summary>
        /// Gets the friendly name of the element.
        /// </summary>
        /// <value>
        /// The element name.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the qualified name of the element.
        /// </summary>
        /// <value>
        /// The qualified name of the element.
        /// </value>
        /// <remarks>
        /// The qualified name is unique within the container's members.
        /// Some elements have the qualified name the same as their name,
        /// but others will use a discriminator prefix to avoid name collisions.
        /// For example, attributes use the "@" discriminator, dimensions use "^", and projections use ":".
        /// </remarks>
        public abstract string QualifiedName { get; }

        /// <summary>
        /// Gets the fully qualified name, starting from the root model space.
        /// </summary>
        /// <value>
        /// The fully qualified name.
        /// </value>
        /// <remarks>
        /// The fully qualified name is built up of qualified names separated by "/".
        /// </remarks>
        /// <example>
        ///   <para>
        /// /: identifies the root model space.
        /// </para>
        ///   <para>
        /// /^AppLayer: identifies the AppLayer dimension.
        /// </para>
        ///   <para>
        /// /:Primitives:Kephas:Core:Main:Global/String: identifies the String classifier within the :Primitives:Kephas:Core:Main:Global projection.
        /// </para>
        ///   <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name: identifies the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection.
        /// </para>
        ///   <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name/@Required: identifies the Required attribute of the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection.
        /// </para>
        /// </example>
        public abstract string FullQualifiedName { get; }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        public abstract IModelElement Container { get; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public abstract IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets the element infos which constructed this element.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public abstract IEnumerable<INamedElementInfo> UnderlyingElementInfos { get; }
    }
}