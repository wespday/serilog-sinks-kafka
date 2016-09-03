// <copyright file="KafkaClient.cs" company="Wes Day">
// Copyright (c) 2016 Wes Day
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Serilog.Sinks.Kafka
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    using kafka4net;

    internal class KafkaClient : AbstractKafkaClient
    {
        internal KafkaClient(KafkaSinkOptions options)
            : base(options)
        {
            Contract.Requires<ArgumentNullException>(options != null);
        }
    }

    internal abstract class AbstractKafkaClient : IDisposable
    {
        private readonly KafkaSinkOptions options;
        private readonly ProducerConfiguration producerConfiguration;
        private Producer kafkaProducer;

        protected AbstractKafkaClient(KafkaSinkOptions options)
        {
            Contract.Requires<ArgumentNullException>(options != null);

            this.options = options;
            this.producerConfiguration = new ProducerConfiguration(options.Topic);
        }

        [ContractVerification(false)]
        public void Dispose()
        {
            var producer = this.kafkaProducer;

            if (producer != null)
            {
                this.kafkaProducer = null;

                producer.CloseAsync(TimeSpan.FromSeconds(10)).Wait();
            }
        }

        internal virtual async Task SendMessagesAsync(IReadOnlyCollection<Message> kafkaMessages)
        {
            Contract.Requires<ArgumentException>(kafkaMessages != null && kafkaMessages.Any());

            var producer = await this.GetOrCreateKafkaProducerAsync().ConfigureAwait(false);

            foreach (var kafkaMessage in kafkaMessages)
            {
                producer.Send(kafkaMessage);
            }
        }

       private async Task<Producer> CreateKafkaProducerAsync()
        {
            // Note: Currently the Producer does not want the protocol passed in
            var connectionString = string.Join(", ", this.options.Brokers.Select(uri => string.IsNullOrEmpty(uri.Authority) ? uri.AbsoluteUri : uri.Authority));

           // ReSharper disable once UseObjectOrCollectionInitializer -- Harder to debug lambda
            var producer = new Producer(connectionString, this.producerConfiguration);

            producer.OnPermError = (exception, messages) =>
            {
                this.kafkaProducer = null;
            };

            producer.OnTempError += messages =>
            {
                Contract.Assume(false, "Kafka Message Delilvery Error");
            };

            await producer.ConnectAsync().ConfigureAwait(false);

            return producer;
        }

        private async Task<Producer> GetOrCreateKafkaProducerAsync()
        {
            // Use a temp producer variable here beacuase this.kafkaProducer could be reset to null in another thread
            var producer = this.kafkaProducer;

            if (producer == null)
            {
                this.kafkaProducer = producer = await this.CreateKafkaProducerAsync().ConfigureAwait(false);
            }

            return producer;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.options != null);
            Contract.Invariant(this.producerConfiguration != null);
        }
    }
}
