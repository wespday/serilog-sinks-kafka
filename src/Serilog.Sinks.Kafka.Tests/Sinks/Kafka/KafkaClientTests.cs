// <copyright file="KafkaClientTests.cs" company="Wes Day">
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

namespace Serilog.Sinks.Kafka.Tests.Sinks.Kafka
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using kafka4net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class KafkaClientTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public async Task KafkaClientConnectionTest()
        {
            // Given
            const string Topic = "test";
            var brokers = new[] { new Uri("localhost:9092") };
            var options = new KafkaSinkOptions(Topic, brokers);
            var kafkaClient = new KafkaClient(options);
            var message = new Message { Value = Encoding.UTF8.GetBytes("{'message' : 'This is a test message'}") };

            // When
            await kafkaClient.SendMessagesAsync(new[] { message }).ConfigureAwait(false);
            kafkaClient.Dispose();

            // Then
            Assert.IsTrue(true);
        }
    }
}
