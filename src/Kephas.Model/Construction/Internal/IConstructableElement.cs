﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConstructableElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IConstructableElement interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Construction.Internal
{
    using System.Collections.Generic;

    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Transitioning;

    /// <summary>
    /// Interface for writable named elements used during construction.
    /// </summary>
    internal interface IConstructableElement
    {
        /// <summary>
        /// Gets the state of the construction.
        /// </summary>
        /// <value>
        /// The construction state.
        /// </value>
        ITransitionState ConstructionState { get; }

        /// <summary>
        /// Sets the element's declaring container.
        /// </summary>
        /// <param name="container">The element's declaring container.</param>
        void SetDeclaringContainer(IModelElement container);

        /// <summary>
        /// Adds the member to the members list.
        /// </summary>
        /// <param name="member">The member.</param>
        void AddMember(INamedElement member);

        /// <summary>
        /// Adds a part to the aggregated element.
        /// </summary>
        /// <param name="part">The part to be added.</param>
        void AddPart(object part);

        /// <summary>
        /// Completes the construction of the element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        void CompleteConstruction(IModelConstructionContext constructionContext);
    }
}