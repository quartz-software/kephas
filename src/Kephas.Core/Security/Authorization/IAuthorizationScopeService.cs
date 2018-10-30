﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthorizationScopeService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAuthorizationScopeService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Interface for authorization scope service.
    /// </summary>
    [SharedAppServiceContract]
    public interface IAuthorizationScopeService
    {
        /// <summary>
        /// Gets the authorization scope asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the authorization scope.
        /// </returns>
        Task<object> GetAuthorizationScopeAsync(IContext context, CancellationToken cancellationToken = default);
    }
}