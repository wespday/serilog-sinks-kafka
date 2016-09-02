// <copyright file="KafkaSinkTests.cs" company="Wes Day">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using kafka4net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NSubstitute;

    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoNSubstitute;

    using Serilog.Events;

    [TestClass]
    public class KafkaSinkTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void CanSendEmptyMessageBatch()
        {
            // Given
            var kafkaClient = new KafkaClient();
            var options = new KafkaSinkOptions("topic", new[] { new Uri("http://sample") });
            var systemUnderTest = new KafkaSink(kafkaClient, options);
            var logEvents = new LogEvent[0];
            
            // When 
            var result = systemUnderTest.EmitBatchInternalAsync(logEvents);

            // Then
            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsFaulted);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task CanSendSingleMessageBatch()
        {
            // Given
            var testFixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var kafkaClient = testFixture.Freeze<AbstractKafkaClient>();
            var logEvents = testFixture.CreateMany<LogEvent>(1).ToArray();
            var systemUnderTest = testFixture.Create<KafkaSink>();

            // When 
            await systemUnderTest.EmitBatchInternalAsync(logEvents).ConfigureAwait(false);

            // Then
            kafkaClient
                .Received(1)
                .SendMessagesAsync(Arg.Is<ICollection<Message>>(val => val.Count == logEvents.Length), Arg.Any<IReadOnlyCollection<Uri>>(), Arg.Any<ProducerConfiguration>());
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task CanSendMultipleMessageBatch()
        {
            // Given
            var testFixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var kafkaClient = testFixture.Freeze<AbstractKafkaClient>();
            var logEvents = testFixture.CreateMany<LogEvent>().ToArray();
            var systemUnderTest = testFixture.Create<KafkaSink>();

            // When 
            await systemUnderTest.EmitBatchInternalAsync(logEvents).ConfigureAwait(false);

            // Then
            kafkaClient
                .Received(1)
                .SendMessagesAsync(Arg.Is<ICollection<Message>>(val => val.Count == logEvents.Length), Arg.Any<IReadOnlyCollection<Uri>>(), Arg.Any<ProducerConfiguration>());
        }
    }
}
