// <copyright file="ProjectionFactory.cs" company="Rolosoft Ltd">
// (c) 2017, Rolosoft Ltd
// </copyright>

namespace SimpleMapper.Projection
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// The projection factory extension methods.
    /// </summary>
    public static class ProjectionFactory
    {
        /// <summary>
        /// Project object.
        /// </summary>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The <see cref="ProjectionExpression&lt;TSource&gt;" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">source is null</exception>
        public static ProjectionExpression<TSource> Project<TSource>([NotNull] this IQueryable<TSource> source)
        {
            Contract.Requires(source != null);

            return new ProjectionExpression<TSource>(source);
        }
    }
}