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
    }

    internal abstract class AbstractKafkaClient
    {
        internal virtual async Task SendMessagesAsync(ICollection<Message> kafkaMessages, IReadOnlyCollection<Uri> brokers, ProducerConfiguration producerConfiguration)
        {
            Contract.Requires<ArgumentException>(kafkaMessages != null && kafkaMessages.Any());
            Contract.Requires<ArgumentNullException>(brokers != null && brokers.Any());
            Contract.Requires<ArgumentNullException>(producerConfiguration != null);

            var connectionString = string.Join(", ", brokers.Select(uri => uri.ToString()));
            var producer = new Producer(connectionString, producerConfiguration);
            await producer.ConnectAsync().ConfigureAwait(false);

            foreach (var kafkaMessage in kafkaMessages)
            {
                producer.Send(kafkaMessage);
            }

            await producer.CloseAsync(TimeSpan.FromSeconds(20)).ConfigureAwait(false);
        }
    }
}
