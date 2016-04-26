// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KafkaSink.cs" company="Wes Day">
//   Copyright (c) 2015 Wes Day
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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using KafkaNet;
    using KafkaNet.Model;
    using KafkaNet.Protocol;

    using PeriodicBatching;

    using Serilog.Events;

    /// <summary>
    /// Writes log events as documents to Kafka.
    /// </summary>
    public class KafkaSink : PeriodicBatchingSink
    {
        private readonly KafkaSinkOptions kafkaSinkOptions;

        private Producer kafkaProducer;

        private string kafkaTopic;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaSink"/> class.
        /// </summary>
        /// <param name="options">
        /// The configuration options.
        /// </param>
        /// <param name="kafkaHostPrimary"> kafka host</param>
        /// <param name="kafkaHostSecondary"> kafka host 2</param>
        /// <param name="kafkaTopic">Kafka topic</param>
        public KafkaSink(KafkaSinkOptions options, string kafkaHostPrimary, string kafkaHostSecondary, string kafkaTopic)
            : base(options.BatchPostingLimit, options.Period)
        {
            Contract.Requires<ArgumentNullException>(options != null);
            Contract.Requires(kafkaHostPrimary != null);
            Contract.Requires(kafkaHostSecondary != null);
            Contract.Requires(kafkaTopic != null);

            this.kafkaSinkOptions = options;
            this.kafkaSinkOptions.BatchPostingLimit = 50;
            this.kafkaSinkOptions.Period = TimeSpan.FromSeconds(5);

            this.kafkaProducer = new Producer(new BrokerRouter(new KafkaOptions(new Uri(kafkaHostPrimary), new Uri(kafkaHostSecondary))));
            this.kafkaTopic = kafkaTopic;
        }

        /// <summary>
        /// Emit a batch of log events, running to completion synchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            Contract.Assume(this.kafkaProducer != null);
            foreach (var logevent in events)
            {
                this.kafkaProducer.SendMessageAsync(this.kafkaTopic, new[] { new Message(logevent.RenderMessage()) }).Wait();    
            }
        }
    }
}
