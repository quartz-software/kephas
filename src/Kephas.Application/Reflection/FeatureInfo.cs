﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the feature information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Provides information about an application feature.
    /// </summary>
    public class FeatureInfo : Expando, IFeatureInfo
    {
        private static readonly System.Version VersionZero = System.Version.Parse("0.0.0.0");

        /// <summary>
        /// The empty annotations collection.
        /// </summary>
        private static readonly IEnumerable<object> EmptyAnnotations = new ReadOnlyCollection<object>(new List<object>());

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfo"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="version">Optional. The feature version.</param>
        /// <param name="isRequired">Optional. True if this feature is required, false if not.</param>
        /// <param name="dependencies">Optional. The feature dependencies.</param>
        public FeatureInfo(string name, string version = null, bool isRequired = false, string[] dependencies = null)
            : this(name, version == null ? null : new Version(version), isRequired, dependencies)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInfo"/> class.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="version">Optional. The feature version.</param>
        /// <param name="isRequired">Optional. True if this feature is required, false if not.</param>
        /// <param name="dependencies">Optional. The feature dependencies.</param>
        public FeatureInfo(string name, Version version = null, bool isRequired = false, string[] dependencies = null)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            this.Name = name;
            this.IsRequired = isRequired;
            this.Version = version ?? new Version(0, 0);
            this.Dependencies = dependencies ?? new string[0];
        }

        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the feature dependencies.
        /// </summary>
        /// <value>
        /// The dependencies.
        /// </value>
        public string[] Dependencies { get; }

        /// <summary>
        /// Gets the feature version.
        /// </summary>
        /// <value>
        /// The feature version.
        /// </value>
        public Version Version { get; }

        /// <summary>
        /// Gets a value indicating whether this feature is required.
        /// </summary>
        /// <value>
        /// True if this feature is required, false if not.
        /// </value>
        public bool IsRequired { get; }

        /// <summary>
        /// Gets the full name of the <see cref="FeatureInfo"/>.
        /// </summary>
        string IElementInfo.FullName => Name;

        /// <summary>
        /// Gets the annotations of the <see cref="FeatureInfo"/>.
        /// </summary>
        IEnumerable<object> IElementInfo.Annotations => EmptyAnnotations;

        /// <summary>
        /// Gets the declaring container of the <see cref="FeatureInfo"/>.
        /// </summary>
        IElementInfo IElementInfo.DeclaringContainer => null;

        /// <summary>
        /// Gets the <see cref="FeatureInfo"/> from the given metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        /// A <see cref="FeatureInfo"/>.
        /// </returns>
        public static FeatureInfo FromMetadata(FeatureManagerMetadata metadata)
        {
            Requires.NotNull(metadata, nameof(metadata));

            if (metadata.FeatureInfo != null)
            {
                return metadata.FeatureInfo;
            }

            var autoVersion = VersionZero;

            var name = metadata.AppServiceImplementationType?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return new FeatureInfo($"unnamed-{Guid.NewGuid()}", autoVersion);
            }

            var wellKnownEndings = new[] { "FeatureManager", "Manager", "AppInitializer", "AppFinalizer" };

            foreach (var ending in wellKnownEndings)
            {
                if (name.EndsWith(ending))
                {
                    var featureName = name.Substring(0, name.Length - ending.Length);
                    if (!string.IsNullOrEmpty(featureName))
                    {
                        return new FeatureInfo(featureName, autoVersion);
                    }
                }
            }

            return new FeatureInfo(name, autoVersion);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var deps = string.Join(", ", Dependencies ?? new string[0]);
            return $"{Name}({deps})";
        }

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        IEnumerable<TAttribute> IAttributeProvider.GetAttributes<TAttribute>() => new TAttribute[0];
    }
}