﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoRoleStoreServiceBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Base for classes implementing <see cref="IRoleStoreService{TRole}"/> with role classes implementing <see cref="IExpando"/>.
    /// </summary>
    /// <typeparam name="TRole">The role type.</typeparam>
    public abstract class ExpandoRoleStoreServiceBase<TRole> : IRoleStoreService<TRole>
        where TRole : class, IExpando
    {
        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public abstract Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public abstract Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public abstract Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken);

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public abstract Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public abstract Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken);

        /// <summary>
        /// Get a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that contains the name of the role.</returns>
        public virtual Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.GetRoleName(role));
        }

        /// <summary>
        /// Gets the ID for a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose ID should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that contains the ID of the role.</returns>
        public virtual Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.GetRoleId(role));
        }

        /// <summary>
        /// Gets the name of a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be returned.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that contains the name of the role.</returns>
        public virtual Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.GetRoleName(role));
        }

        /// <summary>
        /// Set a role's normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose normalized name should be set.</param>
        /// <param name="normalizedName">The normalized name to set</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            this.SetRoleName(role, normalizedName.ToLower());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the name of a role in the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose name should be set.</param>
        /// <param name="roleName">The name of the role.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            this.SetRoleName(role, roleName.ToLower());
            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether was called from <see cref="Dispose"/></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Gets the role ID.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The role ID.</returns>
        protected virtual string GetRoleId(TRole role) => role["Id"] as string;

        /// <summary>
        /// Sets the role ID.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="value">The role ID.</param>
        protected virtual void SetRoleId(TRole role, string value) => role["Id"] = value;

        /// <summary>
        /// Gets the role name.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The role name.</returns>
        protected virtual string GetRoleName(TRole role) => role["Name"] as string;

        /// <summary>
        /// Sets the role name.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="value">The role name.</param>
        protected virtual void SetRoleName(TRole role, string value) => role["Name"] = value;
    }
}