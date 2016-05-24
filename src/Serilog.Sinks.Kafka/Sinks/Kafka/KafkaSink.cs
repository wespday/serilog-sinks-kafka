// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KafkaSink.cs" company="Wes Day">
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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using KafkaNet;
    using KafkaNet.Model;
    using KafkaNet.Protocol;

    using PeriodicBatching;

    using Serilog.Events;
    using Serilog.Formatting.Json;

    /// <summary>
    /// Writes log events as documents to Kafka.
    /// </summary>
    internal class KafkaSink : PeriodicBatchingSink
    {
        private readonly JsonFormatter jsonFormatter;
        private readonly KafkaSinkOptions kafkaSinkOptions;
        private readonly KafkaOptions kafkaOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaSink"/> class.
        /// </summary>
        /// <param name="options">
        /// The configuration options.
        /// </param>
        public KafkaSink(KafkaSinkOptions options)
            : base(options.BatchPostingLimit, options.Period)
        {
            Contract.Requires<ArgumentNullException>(options != null);

            this.kafkaSinkOptions = options;
            this.kafkaOptions = new KafkaOptions(this.kafkaSinkOptions.Brokers);
            this.jsonFormatter = new JsonFormatter(renderMessage: options.RenderSerilogMessage);
        }

        /// <summary>
        /// Emit a batch of log events, running to completion asynchronously.
        /// </summary>
        /// <param name="events">
        /// The events to emit.
        /// </param>
        /// <remarks>
        /// Override either <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatch(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})"/>
        ///  or <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatchAsync(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})"/>,
        /// not both.
        /// </remarks>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
            {
                return;
            }

            var kafkaMessages = events.Select(this.CreateKafkaMessage);
            await this.SendKafkaMessages(kafkaMessages).ConfigureAwait(false);
        }

        private async Task SendKafkaMessages(IEnumerable<Message> kafkaMessages)
        {
            using (var router = new BrokerRouter(this.kafkaOptions))
            using (var kafkaClient = new Producer(router))
            {
                await kafkaClient.SendMessageAsync(this.kafkaSinkOptions.Topic, kafkaMessages).ConfigureAwait(false);
            }
        }

        private Message CreateKafkaMessage(LogEvent loggingEvent)
        {
            Contract.Requires<ArgumentNullException>(loggingEvent != null);
            Contract.Ensures(Contract.Result<Message>() != null);

            using (var eventText = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.jsonFormatter.Format(loggingEvent, eventText);
                var message = new Message(eventText.ToString());
                return message;
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.jsonFormatter != null);
            Contract.Invariant(this.kafkaSinkOptions != null);
            Contract.Invariant(this.kafkaOptions != null);
        }
    }
}
