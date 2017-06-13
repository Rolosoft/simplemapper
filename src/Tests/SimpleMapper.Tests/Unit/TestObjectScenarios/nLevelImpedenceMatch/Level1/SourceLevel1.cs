// <copyright file="SourceLevel1.cs" company="Rolosoft Ltd">
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
namespace SimpleMapper.Tests.Unit.TestObjectScenarios.nLevelImpedenceMatch.Level1
{
    using System.Collections.Generic;
    using Level2;

    /// <summary>
    ///     The source level 1.
    /// </summary>
    public class SourceLevel1
    {
        /// <summary>
        /// Gets or sets the level 1_ id.
        /// </summary>
        public int Level1_Id { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Level1_Name { get; set; }

        /// <summary>
        /// Gets or sets the source level2 objects.
        /// </summary>
        /// <value>
        /// The source level2 objects.
        /// </value>
        public List<SourceLevel2> SourceLevel2Objects { get; set; }
    }
}