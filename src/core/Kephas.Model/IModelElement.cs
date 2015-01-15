﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract providing base information about a model element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract providing base information about a model element.
    /// </summary>
    [ContractClass(typeof(ModelElementContractClass))]
    public interface IModelElement : INamedElement
    {
        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The model element members.
        /// </value>
        IEnumerable<INamedElement> Members { get; }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        IEnumerable<IModelAttribute> Attributes { get; }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        IModelElement Base { get; }

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>The member with the provided qualified name or <c>null</c>.</returns>
        INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true);
    }

    /// <summary>
    /// Contract class for <see cref="IModelElement"/>.
    /// </summary>
    [ContractClassFor(typeof(IModelElement))]
    internal abstract class ModelElementContractClass : IModelElement
    {
        /// <summary>
        /// Gets the members of this model element.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public IEnumerable<INamedElement> Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<INamedElement>>() != null);
                return Contract.Result<IEnumerable<INamedElement>>();
            }
        }

        /// <summary>
        /// Gets the attributes of this model element.
        /// </summary>
        /// <value>
        /// The model element attributes.
        /// </value>
        public IEnumerable<IModelAttribute> Attributes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IModelAttribute>>() != null);
                return Contract.Result<IEnumerable<IModelAttribute>>();
            }
        }

        /// <summary>
        /// Gets the base model element.
        /// </summary>
        /// <value>
        /// The base model element.
        /// </value>
        public IModelElement Base
        {
            get
            {
                return Contract.Result<IModelElement>();
            }
        }

        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
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

        /// <summary>
        /// Gets the member with the specified qualified name.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the member.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> and the member is not found, an exception occurs; otherwise <c>null</c> is returned if the member is not found.</param>
        /// <returns>
        /// The member with the provided qualified name or <c>null</c>.
        /// </returns>
        public INamedElement GetMember(string qualifiedName, bool throwOnNotFound = true)
        {
            Contract.Requires(qualifiedName != null);

            return Contract.Result<INamedElement>();
        }
    }

    /// <summary>
    /// Extensions for <see cref="IModelElement"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class ModelElementExtensions
    {
        /// <summary>
        /// Gets the model element's own members, excluding those declared by the base element or mixins.
        /// </summary>
        /// <param name="modelElement">The model element.</param>
        /// <returns>The members declared exclusively at the element level.</returns>
        public static IEnumerable<INamedElement> GetOwnMembers(this IModelElement modelElement)
        {
            Contract.Requires(modelElement != null);

            return modelElement.Members.Where(m => m.Container == modelElement);
        }
    }
}