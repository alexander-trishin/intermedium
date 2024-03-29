﻿using System.Threading;
using System.Threading.Tasks;

namespace Intermedium.Pipeline
{
    /// <summary>
    /// Serves as the base class for classes that synchronously performs
    /// post-processing of a <typeparamref name="TQuery"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public abstract class QueryPostProcessor<TQuery, TResult> : IQueryPostProcessor<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task IQueryPostProcessor<TQuery, TResult>.ProcessAsync(
            TQuery query,
            IPostProcessorContext<TResult> context,
            CancellationToken cancellationToken)
        {
            Process(query, context, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes an action after <typeparamref name="TQuery"/> was handled.
        /// </summary>
        /// <param name="query">The query to <see cref="IMediatorSender"/>.</param>
        /// <param name="context">
        /// The context that contains the result of <paramref name="query"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        public abstract void Process(
            TQuery query,
            IPostProcessorContext<TResult> context,
            CancellationToken cancellationToken
        );
    }
}
