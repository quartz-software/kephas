﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecuteCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IExecuteCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for data commands executing commands on the data store side.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IExecuteCommand : IDataCommand<IExecuteContext, IExecuteResult>
    {
    }
}