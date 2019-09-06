﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionServiceSource.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the collection service source class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas;
    using Kephas.Reflection;

    internal class CollectionServiceSource : ServiceSourceBase
    {
        private static readonly MethodInfo GetServiceMethod =
            ReflectionHelper.GetGenericMethodOf(_ => GetService<string>(null, null));

        public CollectionServiceSource(IServiceRegistry registry)
            : base(registry)
        {
        }

        public override bool IsMatch(Type contractType)
        {
            return contractType.IsConstructedGenericOf(typeof(ICollection<>));
        }

        public override IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> GetServiceDescriptors(
            IAmbientServices parent,
            Type serviceType)
        {
            var innerType = serviceType.GetGenericArguments()[0];
            return GetServiceDescriptors(parent, innerType, null);
        }

        public override object GetService(IAmbientServices parent, Type serviceType)
        {
            var descriptors = GetServiceDescriptors(parent, serviceType);
            var itemType = serviceType.GetGenericArguments()[0];
            var getService = GetServiceMethod.MakeGenericMethod(itemType);
            return getService.Call(null, parent, descriptors);
        }

        private static ICollection<T> GetService<T>(IServiceProvider parent, IEnumerable<(IServiceInfo serviceInfo, Func<object> factory)> descriptors)
            where T : class
        {
            return descriptors.Select(d => (T)d.factory()).ToList();
        }
    }
}