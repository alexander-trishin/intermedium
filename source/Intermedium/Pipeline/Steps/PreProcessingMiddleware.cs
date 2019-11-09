﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Core.Internal;

namespace Intermedium.Pipeline.Steps
{
    /// <summary>
    /// The default <see cref="IMiddleware{TQuery, TResult}"/> for request pre-processing.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public sealed class PreProcessingMiddleware<TQuery, TResult> : IMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IEnumerable<IQueryPreProcessor<TQuery, TResult>> _processors;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PreProcessingMiddleware{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="processors">The list of request pre-processors.</param>
        /// <param name="comparer">
        /// The comparer to control the execution order of pre-processors.
        /// </param>
        public PreProcessingMiddleware(
            IEnumerable<IQueryPreProcessor<TQuery, TResult>> processors,
            IComparer<IQueryPreProcessor<TQuery, TResult>> comparer)
        {
            _processors = processors
                .EmptyIfNull()
                .Sort(comparer)
                .ToList();
        }

        /// <summary>
        /// Executes the current component in the pipeline.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediator"/>.</param>
        /// <param name="nextAsync">The request handler.</param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the work.
        /// </param>
        /// <returns>A task that represents a pipeline step.</returns>
        public async Task<TResult> ExecuteAsync(
            TQuery request,
            Func<Task<TResult>> nextAsync,
            CancellationToken cancellationToken)
        {
            foreach (var processor in _processors)
            {
                await processor.ProcessAsync(request, cancellationToken).ConfigureAwait(false);
            }

            return await nextAsync().ConfigureAwait(false);
        }
    }
}
