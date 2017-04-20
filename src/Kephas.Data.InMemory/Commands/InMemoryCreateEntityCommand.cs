﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryCreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client create entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Create entity command implementation for a <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryCreateEntityCommand : CreateEntityCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryCreateEntityCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public InMemoryCreateEntityCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
            Requires.NotNull(behaviorProvider, nameof(behaviorProvider));
        }

        /// <summary>
        /// Tries to get the data context's local cache.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IDataContextCache.
        /// </returns>
        protected override IDataContextCache TryGetLocalCache(IDataContext dataContext)
        {
            var inMemoryDataContext = (InMemoryDataContext)dataContext;
            return inMemoryDataContext.WorkingCache;
        }
    }
}