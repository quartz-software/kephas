﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryIdentityRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// In-memory identity repository.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class InMemoryIdentityRepository : IInMemoryIdentityRepository
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> repository = new ();

        /// <summary>
        /// Gets the query over a certain type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The query.</returns>
        public IQueryable<T> Query<T>()
        {
            var repo = this.repository.GetOrAdd(typeof(T), _ => new ());
            return repo.Values.Cast<T>().AsQueryable();
        }

        /// <summary>
        /// Creates a new item in a store as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item to create in the store.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public Task<IdentityResult> CreateAsync<T>(T item, string id, CancellationToken cancellationToken)
        {
            var repo = this.repository.GetOrAdd(typeof(T), _ => new ());
            return Task.FromResult(repo.TryAdd(id, item)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"{typeof(T).Name} with ID '{id}' already added." }));
        }

        /// <summary>
        /// Updates the specified <paramref name="item" /> in the store.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public Task<IdentityResult> UpdateAsync<T>(T item, string id, CancellationToken cancellationToken)
        {
            var repo = this.repository.GetOrAdd(typeof(T), _ => new ());
            return Task.FromResult(repo.TryUpdate(id, item, item)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"{typeof(T).Name} with ID '{id}' not found." }));
        }

        /// <summary>
        /// Deletes the specified <paramref name="item" /> from the store.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public Task<IdentityResult> DeleteAsync<T>(T item, string id, CancellationToken cancellationToken)
        {
            var repo = this.repository.GetOrAdd(typeof(T), _ => new ());
            return Task.FromResult(repo.TryRemove(id, out _)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"{typeof(T).Name} with ID '{id}' not found." }));
        }

        /// <summary>
        /// Tries to find the item by identifier.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public Task<T?> FindByIdAsync<T>(string id, CancellationToken cancellationToken)
        {
            var repo = this.repository.GetOrAdd(typeof(T), _ => new ());
            repo.TryGetValue(id, out var item);
            return Task.FromResult((T?)item);
        }
    }
}