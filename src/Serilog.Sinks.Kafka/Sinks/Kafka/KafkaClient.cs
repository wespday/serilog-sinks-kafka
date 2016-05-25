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

    using KafkaNet;
    using KafkaNet.Model;
    using KafkaNet.Protocol;

    internal class KafkaClient
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "To be made virtual for testing purposes")]
        internal async Task SendMessagesAsync(ICollection<Message> kafkaMessages, string kafkaTopic, KafkaOptions kafkaOptions)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(kafkaTopic));
            Contract.Requires<ArgumentException>(kafkaMessages != null && kafkaMessages.Any());
            Contract.Requires<ArgumentNullException>(kafkaOptions != null);

            using (var router = new BrokerRouter(kafkaOptions))
            using (var kafkaClient = new Producer(router))
            {
                await kafkaClient.SendMessageAsync(kafkaTopic, kafkaMessages).ConfigureAwait(false);
            }
        }
    }
}
