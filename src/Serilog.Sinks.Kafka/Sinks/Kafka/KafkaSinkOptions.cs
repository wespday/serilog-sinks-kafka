// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KafkaSinkOptions.cs" company="Wes Day">
//   Copyright (c) 2016 Wes Day
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//          http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Kafka
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The kafka sink options.
    /// </summary>
    public class KafkaSinkOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of events to post in a single batch.
        /// </summary>
        public int BatchPostingLimit { get; set; } = 50;

        /// <summary>
        /// Gets or sets the time to wait between checking for event batches. Defaults to 2 seconds.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Gets or sets a list of Kafka URIs.
        /// </summary>
        public Uri[] KafkaUris { get; set; } = new Uri[0];

        /// <summary>
        /// Gets or sets the name of the Kafka queue to write to.
        /// </summary>
        public string KafkaTopicName { get; set; } = string.Empty;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.KafkaUris != null);
            Contract.Invariant(this.KafkaTopicName != null);
        }
    }
}
