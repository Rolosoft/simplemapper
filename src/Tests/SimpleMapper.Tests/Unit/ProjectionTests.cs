// <copyright file="ProjectionTests.cs" company="Rolosoft Ltd">
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
namespace SimpleMapper.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;
    using Projection;
    using TestObjectScenarios.FlatImpedenceMismath;
    using TestObjectScenarios.nLevelImpedenceMatch.Level1;
    using TestObjectScenarios.nLevelImpedenceMatch.Level2;
    using Xunit;
    using Xunit.Abstractions;
    using Destination = TestObjectScenarios.FlatImpedenceMatch.Destination;
    using Source = TestObjectScenarios.FlatImpedenceMatch.Source;

    /// <summary>
    /// The projection tests.
    /// </summary>
    public class ProjectionTests : TestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionTests"/> class.
        /// </summary>
        /// <param name="outHelper">The out helper.</param>
        public ProjectionTests([NotNull] ITestOutputHelper outHelper)
            : base(outHelper)
        {
        }

        /// <summary>
        /// The project_ when flat with impedence match_ expect all attributes mapped.
        /// </summary>
        [Fact]
        public void Project_WhenFlatWithImpedenceMatch_ExpectAllAttributesMapped()
        {
            // arrange
            var sources =
                new List<Source>
                    {
                        new Source { FirstName = @"first1", LastName = @"last1", Id = 1 },
                        new Source { FirstName = @"first2", LastName = @"last2", Id = 2 }
                    }.AsQueryable();

            // act
            for (int index = 0; index < 100; index++)
            {
                var stopwatch = Stopwatch.StartNew();
                var destinations = sources.Project().To<Destination>();
                stopwatch.Stop();

                // assert
                Assert.NotNull(destinations);
                Assert.True(destinations.Count() == 2);
                Assert.True(string.Compare(sources.FirstOrDefault().FirstName, destinations.FirstOrDefault().FirstName, StringComparison.CurrentCultureIgnoreCase) == 0);
                Assert.True(string.Compare(sources.FirstOrDefault().LastName, destinations.FirstOrDefault().LastName, StringComparison.CurrentCultureIgnoreCase) == 0);
                Assert.True(sources.FirstOrDefault().Id == destinations.FirstOrDefault().Id);

                this.WriteTimeElaped(stopwatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// The project_ when flat with impedence mis match_ expect some attributes mapped.
        /// </summary>
        [Fact]
        public void Project_WhenFlatWithImpedenceMisMatch_ExpectSomeAttributesMapped()
        {
            // arrange
            var sources =
                new List<TestObjectScenarios.FlatImpedenceMismath.Source>
                    {
                        new TestObjectScenarios.FlatImpedenceMismath.Source
                            {
                                FirstName
                                    =
                                    @"first1",
                                LastName
                                    =
                                    @"last1",
                                Id = 1
                            },
                        new TestObjectScenarios.FlatImpedenceMismath.Source
                            {
                                FirstName
                                    =
                                    @"first2",
                                LastName
                                    =
                                    @"last2",
                                Id = 2
                            }
                    }
                    .AsQueryable();

            for (int i = 0; i < 100; i++)
            {
                // act
                var stopwatch = Stopwatch.StartNew();
                var destinations = sources.Project().To<TestObjectScenarios.FlatImpedenceMismath.Destination>();
                stopwatch.Stop();

                // assert
                Assert.NotNull(destinations);
                Assert.True(destinations.Count() == 2);
                Assert.True(string.Compare(sources.FirstOrDefault().FirstName, destinations.FirstOrDefault().FirstNameQQQ, StringComparison.CurrentCultureIgnoreCase) != 0);
                Assert.True(string.Compare(sources.FirstOrDefault().LastName, destinations.FirstOrDefault().LastName, StringComparison.CurrentCultureIgnoreCase) == 0);
                Assert.True(sources.FirstOrDefault().Id != destinations.FirstOrDefault().IdQQQ);

                this.WriteTimeElaped(stopwatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// The project_ when flat with no match_ expect all not mapped.
        /// </summary>
        [Fact]
        public void Project_WhenFlatWithNoMatch_ExpectAllNotMapped()
        {
            // arrange
            var sources =
                new List<NoMatch>
                    {
                        new NoMatch
                            {
                                XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxxx = 123,
                                YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYxx = "fff"
                            },
                        new NoMatch
                            {
                                XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXxxx = 456,
                                YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYxx = "erre"
                            }
                    }.AsQueryable();

            // act
            var stopwatch = Stopwatch.StartNew();
            var destinations = sources.Project().To<TestObjectScenarios.FlatImpedenceMismath.Destination>();
            stopwatch.Stop();

            // assert
            Assert.NotNull(destinations);
            Assert.True(destinations.Count() == 2);
            Assert.Null(destinations.FirstOrDefault().FirstNameQQQ);
            Assert.Null(destinations.FirstOrDefault().LastName);
            Assert.True(destinations.FirstOrDefault().IdQQQ == 0);
            this.WriteTimeElaped(stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// The project_ whenn level level 0 match_ expect level 0 match only.
        /// </summary>
        [Fact]
        public void Project_WhenLevelLevel0Match_ExpectLevel0MatchOnly()
        {
            // arrange
            var source = new List<TestObjectScenarios.nLevelImpedenceMatch.Source>
                             {
                                 new TestObjectScenarios.nLevelImpedenceMatch.Source
                                     {
                                         Id
                                             =
                                             1,
                                         Name
                                             =
                                             @"item1_level0",
                                         SourceLevel1s
                                             =
                                             new List
                                             <
                                             SourceLevel1
                                             >
                                                 {
                                                     new SourceLevel1
                                                         {
                                                             Level1_Id
                                                                 =
                                                                 1,
                                                             Level1_Name
                                                                 =
                                                                 @"item1_level1",
                                                             SourceLevel2Objects
                                                                 =
                                                                 new List
                                                                 <
                                                                 SourceLevel2
                                                                 >
                                                                     {
                                                                         new SourceLevel2
                                                                             {
                                                                                 Level2_Id
                                                                                     =
                                                                                     1,
                                                                                 Level2_Name
                                                                                     =
                                                                                     @"item1_level2"
                                                                             }
                                                                     }
                                                         }
                                                 }
                                     },
                                 new TestObjectScenarios.nLevelImpedenceMatch.Source
                                     {
                                         Id
                                             =
                                             2,
                                         Name
                                             =
                                             @"item2_level0",
                                         SourceLevel1s
                                             =
                                             new List
                                             <
                                             SourceLevel1
                                             >
                                                 {
                                                     new SourceLevel1
                                                         {
                                                             Level1_Id
                                                                 =
                                                                 2,
                                                             Level1_Name
                                                                 =
                                                                 @"item2_level1",
                                                             SourceLevel2Objects
                                                                 =
                                                                 new List
                                                                 <
                                                                 SourceLevel2
                                                                 >
                                                                     {
                                                                         new SourceLevel2
                                                                             {
                                                                                 Level2_Id
                                                                                     =
                                                                                     2,
                                                                                 Level2_Name
                                                                                     =
                                                                                     @"item2_level2"
                                                                             }
                                                                     }
                                                         }
                                                 }
                                     }
                             }.AsQueryable();

            for (int i = 0; i < 100; i++)
            {
                // act
                var stopwatch = Stopwatch.StartNew();
                var destinations = source.Project().To<TestObjectScenarios.nLevelImpedenceMatch.Destination>();
                stopwatch.Stop();

                // assert
                Assert.NotNull(destinations);
                Assert.True(destinations.Count() == 2);
                Assert.Null(destinations.FirstOrDefault().SourceLevel1s);
                Assert.True(string.Compare(@"item1_level0", destinations.FirstOrDefault().Name, StringComparison.CurrentCultureIgnoreCase) == 0);
                Assert.True(destinations.FirstOrDefault().Id == 1);
                this.WriteTimeElaped(stopwatch.ElapsedMilliseconds);
            }
        }
    }
}