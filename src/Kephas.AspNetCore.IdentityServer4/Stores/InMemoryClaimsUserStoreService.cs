﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryClaimsUserStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// An in-memory service for storing <see cref="ClaimsIdentity"/> users.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InMemoryClaimsUserStoreService : InMemoryClaimsUserStoreService<ClaimsIdentity>
    {
    }

    /// <summary>
    /// An in-memory service for storing <see cref="ClaimsIdentity"/> based users.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    public class InMemoryClaimsUserStoreService<TUser> : ClaimsUserStoreServiceBase<TUser>
        where TUser : ClaimsIdentity
    {
        private readonly ConcurrentDictionary<string, TUser> usersById = new ConcurrentDictionary<string, TUser>();

        /// <summary>
        /// Creates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the creation operation.</returns>
        public override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var id = this.GetUserId(user);
            return Task.FromResult(this.usersById.TryAdd(id, user)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"User with ID '{id}' already added." }));
        }

        /// <summary>
        /// Updates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var id = this.GetUserId(user);
            return Task.FromResult(this.usersById.TryUpdate(id, user, user)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"User with ID '{id}' not found." }));
        }

        /// <summary>
        /// Deletes the specified <paramref name="user" /> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            var id = this.GetUserId(user);
            return Task.FromResult(this.usersById.TryRemove(id, out _)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"User with ID '{id}' not found." }));
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId" /> if it exists.
        /// </returns>
        public override Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = this.usersById.TryGetValue(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{userId}' not found.");
            }

            return Task.FromResult(user);
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName" /> if it exists.
        /// </returns>
        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = this.usersById.Values.FirstOrDefault(u => normalizedUserName.Equals(this.GetUserName(u), StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with name '{normalizedUserName}' not found.");
            }

            return Task.FromResult(user);
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = this.usersById.Values.FirstOrDefault(u => normalizedEmail.Equals(this.GetUserEmail(u), StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{normalizedEmail}' not found.");
            }

            return Task.FromResult(user);
        }

        /// <summary>
        /// Returns a list of users who contain the specified <see cref="T:System.Security.Claims.Claim" />.
        /// </summary>
        /// <param name="claim">The claim to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the result of the asynchronous query, a list of <typeparamref name="TUser" /> who
        /// contain the specified claim.
        /// </returns>
        public override Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            IList<TUser> users = this.usersById.Values
                .Where(u => u.HasClaim(claim.Type, claim.Value))
                .ToList();
            return Task.FromResult(users);
        }
    }
}