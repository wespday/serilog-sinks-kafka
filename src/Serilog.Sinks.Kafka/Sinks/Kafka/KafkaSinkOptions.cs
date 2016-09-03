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
    using System.Linq;

    /// <summary>
    /// The kafka sink options.
    /// </summary>
    public class KafkaSinkOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaSinkOptions"/> class.
        /// </summary>
        /// <param name="topic">The Kafka topic to log to</param>
        /// <param name="brokers">The Kafka broker endpionts to log to</param>
        public KafkaSinkOptions(string topic, Uri[] brokers)
        {
            Contract.Requires<ArgumentException>(topic != null);
            Contract.Requires<ArgumentException>(brokers != null && brokers.Any());

            this.Topic = topic;
            this.Brokers = brokers;
        }

        /// <summary>
        /// Gets or sets the maximum number of events to post in a single batch.
        /// </summary>
        public int BatchPostingLimit { get; set; } = 50;

        /// <summary>
        /// Gets or sets the time to wait between checking for event batches.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets a list of Kafka Broker URIs.
        /// </summary>
        public Uri[] Brokers { get; private set; }

        /// <summary>
        /// Gets the name of the Kafka queue to write to.
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Serilog message should be rendered.
        /// </summary>
        public bool RenderSerilogMessage { get; set; } = true;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.Brokers != null && this.Brokers.Any());
            Contract.Invariant(this.Topic != null);
        }
    }
}
