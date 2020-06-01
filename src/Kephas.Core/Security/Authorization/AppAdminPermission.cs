﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppAdminPermission.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using Kephas.Security.Authorization.AttributedModel;

    /// <summary>
    /// Defines the global application administration permission.
    /// </summary>
    [PermissionInfo(TokenName, Scoping.Global)]
    public sealed class AppAdminPermission : IPermission
    {
        /// <summary>
        /// The name of the AppAdmin permission.
        /// </summary>
        public const string TokenName = "appadmin";

        private AppAdminPermission()
        {
        }
    }
}