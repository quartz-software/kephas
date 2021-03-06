﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierConstructorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base runtime provider for classifier information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Construction
{
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Construction;
    using Kephas.Model.Elements;
    using Kephas.Model.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Base runtime provider for classifier information.
    /// </summary>
    /// <typeparam name="TModel">The type of the element information.</typeparam>
    /// <typeparam name="TModelContract">Type of the model contract.</typeparam>
    public abstract class ClassifierConstructorBase<TModel, TModelContract> : ModelElementConstructorBase<TModel, TModelContract, IRuntimeTypeInfo>
        where TModel : ClassifierBase<TModelContract>
        where TModelContract : class, IClassifier
    {
        /// <summary>
        /// Determines whether a model element can be created for the provided runtime element.
        /// </summary>
        /// <param name="constructionContext">Context for the construction.</param>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>
        /// <c>true</c> if a model element can be created, <c>false</c> if not.
        /// </returns>
        protected override bool CanCreateModelElement(IModelConstructionContext constructionContext, IRuntimeTypeInfo runtimeElement)
        {
            var classifierKind = runtimeElement.GetClassifierKind();
            if (classifierKind != null)
            {
                return classifierKind == typeof(TModelContract);
            }

            return false;
        }

        /// <summary>
        /// Computes the model element name based on the runtime element.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <param name="constructionContext">The construction context.</param>
        /// <returns>The element name, or <c>null</c> if the name could not be computed.</returns>
        protected override string? TryComputeNameCore(
            object runtimeElement,
            IModelConstructionContext constructionContext)
        {
            if (runtimeElement is IRuntimeTypeInfo typeInfo)
            {
                var kindAttr = typeInfo.GetAttribute<ClassifierKindAttribute>();
                if (kindAttr != null && !string.IsNullOrEmpty(kindAttr.ClassifierName))
                {
                    return kindAttr.ClassifierName;
                }
            }

            return base.TryComputeNameCore(runtimeElement, constructionContext);
        }
    }
}