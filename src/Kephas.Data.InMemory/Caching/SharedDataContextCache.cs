﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedDataContextCache.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the shared data context cache class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Caching
{
    using System.Collections.Concurrent;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;

    /// <summary>
    /// A shared data context cache.
    /// </summary>
    public class SharedDataContextCache : ConcurrentDictionary<Id, IEntityInfo>, IDataContextCache
    {
    }
}