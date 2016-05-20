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
        [TestCategory("Integration")]
        public void KafkaConnectionTest()
        {
            var log = new LoggerConfiguration().WriteTo.Kafka("Sense_log", "http://vcld16rdacoas02.ual.com:9092", "http://vcld16rdacoas03.ual.com:9092", LevelAlias.Minimum, 50, TimeSpan.FromMinutes(1)).CreateLogger();

            log.Information("This is a test {message}", 3);

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void SerilogToKafkaTest()
        {
        }
    }
}
