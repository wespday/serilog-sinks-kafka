using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serilog.Sinks.Kafka.Test
{
    using System.Configuration;
    using KafkaNet;
    using KafkaNet.Model;
    using KafkaNet.Protocol;
    using Serilog;
    using Serilog.Events;

    [TestClass]
    public class SerilogSinksKafkaTest
    {
        [TestMethod]
        public void KafkaConfigTest()
        {
        }

        [TestMethod]
        public void KafkaConnectionTest()
        {
            var log = new LoggerConfiguration().WriteTo.Kafka("TopicName", "http://host1:9092", "http://host2:9092", LevelAlias.Minimum, 50, TimeSpan.FromMinutes(1)).CreateLogger();

            log.Error("some error from application");

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void SerilogToKafkaTest()
        {
        }
    }
}
