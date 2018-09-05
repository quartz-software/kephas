﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoQuery.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo query class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Linq
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;

    using global::MongoDB.Driver;
    using global::MongoDB.Driver.Linq;

    /// <summary>
    /// A mongo query adapter.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class MongoQuery<T> : DataContextQuery<T>, IMongoQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoQuery{T}"/> class.
        /// </summary>
        /// <param name="queryProvider">The query provider.</param>
        /// <param name="nativeQuery">The native query.</param>
        public MongoQuery(MongoQueryProvider queryProvider, IMongoQueryable<T> nativeQuery)
            : base(queryProvider, nativeQuery)
        {
        }

        /// <summary>
        /// Gets the execution model.
        /// </summary>
        /// <returns>
        /// The execution model.
        /// </returns>
        public QueryableExecutionModel GetExecutionModel()
        {
            return this.GetNativeExecutableQuery().GetExecutionModel();
        }

        /// <summary>
        /// Executes the operation and returns a cursor to the results.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A cursor.</returns>
        public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = default)
        {
            return this.GetNativeExecutableQuery().ToCursor(cancellationToken);
        }

        /// <summary>
        /// Executes the operation and returns a cursor to the results.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is a cursor.</returns>
        public Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            return this.GetNativeExecutableQuery().ToCursorAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the native executable query.
        /// </summary>
        /// <returns>
        /// The native executable query.
        /// </returns>
        protected virtual IMongoQueryable<T> GetNativeExecutableQuery()
        {
            var mongoQueryProvider = (MongoQueryProvider)this.Provider;
            var executableQuery = mongoQueryProvider.NativeQueryProvider.CreateQuery(mongoQueryProvider.GetExecutableExpression(this.Expression));
            return (IMongoQueryable<T>)executableQuery;
        }
    }
}