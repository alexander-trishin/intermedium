﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intermedium.Pipeline.Steps.Internal;

namespace Intermedium.Pipeline.Steps
{
    /// <summary>
    /// The default <see cref="IMiddleware{TQuery, TResult}"/> for request post-processing.
    /// </summary>
    /// <typeparam name="TQuery">The type of a query.</typeparam>
    /// <typeparam name="TResult">
    /// The type of the return value of a <typeparamref name="TQuery"/>.
    /// </typeparam>
    public sealed class PostProcessingMiddleware<TQuery, TResult> : IMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IEnumerable<IQueryPostProcessor<TQuery, TResult>> _processors;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="PostProcessingMiddleware{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="processors">The list of request post-processors.</param>
        public PostProcessingMiddleware(IEnumerable<IQueryPostProcessor<TQuery, TResult>> processors)
        {
            _processors = processors ?? Enumerable.Empty<IQueryPostProcessor<TQuery, TResult>>();
        }

        /// <summary>
        /// Executes the current component in the pipeline.
        /// </summary>
        /// <param name="request">The request to <see cref="IMediatorSender"/>.</param>
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
            var comparer = new ProcessorComparer<IQueryPostProcessor<TQuery, TResult>>();
            var context = new PostProcessorContext<TResult>
            {
                Result = await nextAsync().ConfigureAwait(false)
            };

            foreach (var processor in _processors.OrderBy(x => x, comparer))
            {
                await processor.ProcessAsync(request, context, cancellationToken).ConfigureAwait(false);
            }

            return context.Result;
        }
    }
}
