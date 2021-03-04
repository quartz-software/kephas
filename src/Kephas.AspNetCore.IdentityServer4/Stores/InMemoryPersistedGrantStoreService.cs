﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryPersistedGrantStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using global::IdentityServer4.Stores;
    using Kephas.Services;

    /// <summary>
    /// Application service for in-memory persisted grant store.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InMemoryPersistedGrantStoreService : InMemoryPersistedGrantStore, IPersistedGrantStoreService
    {
    }
}