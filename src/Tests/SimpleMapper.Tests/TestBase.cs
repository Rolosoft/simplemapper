// <copyright file="TestBase.cs" company="Rolosoft Ltd">
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
namespace SimpleMapper.Tests
{
    using System;
    using JetBrains.Annotations;
    using Xunit.Abstractions;

    /// <summary>
    /// Test base.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// The out helper
        /// </summary>
        [NotNull]
        protected readonly ITestOutputHelper OutHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase"/> class.
        /// </summary>
        /// <param name="outHelper">The out helper.</param>
        protected TestBase([NotNull] ITestOutputHelper outHelper)
        {
            this.OutHelper = outHelper;
        }

        /// <summary>
        /// The write time elaped.
        /// </summary>
        /// <param name="timerElapsed">
        /// The timer elapsed.
        /// </param>
        protected void WriteTimeElaped(long timerElapsed)
        {
            this.OutHelper.WriteLine("Elapsed timer: {0}ms", timerElapsed);
        }
    }
}