// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerConfigurationKafkaExtensionsTests.cs" company="Wes Day">
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
// <summary>
//   Defines the SerilogSinksKafkaTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Kafka.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serilog;

    [TestClass]
    public class LoggerConfigurationKafkaExtensionsTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void KafkaConnectionTest()
        {
            var log = new LoggerConfiguration()
                .WriteTo
                .Kafka(new KafkaSinkOptions(topic: "test", brokers: new[] { new Uri("http://localhost:9092") }))
                .CreateLogger();

            log.Information(
                "The execution time of this test is  {@now}",
                new { DateTimeOffset.Now.Hour, DateTimeOffset.Now.Minute });

            log.CloseAndFlush();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CanCreateKafkaSink()
        {
            // Given
            const string Topic = "SomeKafkaTopic";

            var brokers = new[]
            {
                new Uri("http://localhost:3333"),
                new Uri("http://localhost:4444"),
                new Uri("http://localhost:5555")
            };

            // When 
            var log = new LoggerConfiguration().WriteTo.Kafka(new KafkaSinkOptions(Topic, brokers));

            // Then
            Assert.IsNotNull(log);
        }
    }
}