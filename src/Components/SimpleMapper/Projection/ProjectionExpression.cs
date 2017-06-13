// <copyright file="ProjectionExpression.cs" company="Rolosoft Ltd">
// (c) 2017, Rolosoft Ltd
// </copyright>

// Copyright 2017 Rolosoft Ltd
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace SimpleMapper.Projection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// The projection expression.
    /// </summary>
    /// <typeparam name="TSource">
    /// Type of source.
    /// </typeparam>
    public sealed class ProjectionExpression<TSource>
    {
        /// <summary>
        /// The memory cache.
        /// </summary>
        private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        ///     The _source.
        /// </summary>
        private readonly IQueryable<TSource> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionExpression{TSource}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ProjectionExpression([NotNull] IQueryable<TSource> source)
        {
            Contract.Requires(source != null);

            this.source = source;
        }

        /// <summary>
        ///     The to.
        /// </summary>
        /// <typeparam name="TDest">
        /// Type of destination.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IQueryable" />.
        /// </returns>
        public IQueryable<TDest> To<TDest>()
        {
            var queryExpression = GetCachedExpression<TDest>() ?? BuildExpression<TDest>();

            return this.source.Select(queryExpression);
        }

        /// <summary>
        /// The build binding.
        /// </summary>
        /// <param name="parameterExpression">
        /// The parameter expression.
        /// </param>
        /// <param name="destinationProperty">
        /// The destination property.
        /// </param>
        /// <param name="sourceProperties">
        /// The source properties.
        /// </param>
        /// <returns>
        /// The <see cref="MemberAssignment"/>.
        /// </returns>
        private static MemberAssignment BuildBinding(
            Expression parameterExpression,
            MemberInfo destinationProperty,
            IEnumerable<PropertyInfo> sourceProperties)
        {
            var propertyInfos = sourceProperties as IList<PropertyInfo> ?? sourceProperties.ToList();
            var sourceProperty = propertyInfos.FirstOrDefault(src => src.Name == destinationProperty.Name);

            if (sourceProperty != null)
            {
                MemberAssignment rtn;
                try
                {
                   rtn = Expression.Bind(destinationProperty, Expression.Property(parameterExpression, sourceProperty));
                }
                catch (ArgumentException)
                {
                    rtn = default(MemberAssignment);
                }

                return rtn;
            }

            var propertyNames = SplitCamelCase(destinationProperty.Name);

            if (propertyNames.Length == 2)
            {
                sourceProperty = propertyInfos.FirstOrDefault(src => src.Name == propertyNames[0]);

                var sourceChildProperty =
                    sourceProperty?.PropertyType.GetRuntimeProperties().FirstOrDefault(src => src.Name == propertyNames[1]);

                if (sourceChildProperty != null)
                {
                    return Expression.Bind(
                        destinationProperty,
                        Expression.Property(
                            Expression.Property(parameterExpression, sourceProperty),
                            sourceChildProperty));
                }
            }

            return null;
        }

        /// <summary>
        ///     The build expression.
        /// </summary>
        /// <typeparam name="TDest">
        /// Type of destination.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        private static Expression<Func<TSource, TDest>> BuildExpression<TDest>()
        {
            var sourceProperties = typeof(TSource).GetRuntimeProperties();
            var destinationProperties = typeof(TDest).GetRuntimeProperties().Where(dest => dest.CanWrite);
            var parameterExpression = Expression.Parameter(typeof(TSource), "src");

            var bindings =
                destinationProperties.Select(
                    destinationProperty => BuildBinding(parameterExpression, destinationProperty, sourceProperties))
                    .Where(binding => binding != null);

            var expression =
                Expression.Lambda<Func<TSource, TDest>>(
                    Expression.MemberInit(Expression.New(typeof(TDest)), bindings),
                    parameterExpression);

            var key = GetCacheKey<TDest>();

            /*No timeout*/
            MemoryCache.Set(key, expression, DateTimeOffset.MaxValue);

            return expression;
        }

        /// <summary>
        ///     The get cache key.
        /// </summary>
        /// <typeparam name="TDest">
        /// Type of destination.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string GetCacheKey<TDest>()
        {
            return string.Concat(typeof(TSource).FullName, typeof(TDest).FullName);
        }

        /// <summary>
        ///     The get cached expression.
        /// </summary>
        /// <typeparam name="TDest">
        /// Type of destination.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="Expression" />.
        /// </returns>
        private static Expression<Func<TSource, TDest>> GetCachedExpression<TDest>()
        {
            var key = GetCacheKey<TDest>();

            return MemoryCache.Get(key) == null ? null : MemoryCache.Get(key) as Expression<Func<TSource, TDest>>;
        }

        /// <summary>
        /// The split camel case.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] SplitCamelCase(string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim().Split(' ');
        }
    }
}