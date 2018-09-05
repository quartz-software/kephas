﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassifierKindAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the classifier kind attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute for classifier kind.
    /// </summary>
    public abstract class ClassifierKindAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassifierKindAttribute"/> class.
        /// </summary>
        /// <param name="classifierType">The type of the classifier.</param>
        protected ClassifierKindAttribute(Type classifierType)
        {
            Requires.NotNull(classifierType, nameof(classifierType));

            this.ClassifierType = classifierType;
        }

        /// <summary>
        /// Gets the type of the classifier.
        /// </summary>
        /// <value>
        /// The type of the classifier.
        /// </value>
        public Type ClassifierType { get; }
    }
}