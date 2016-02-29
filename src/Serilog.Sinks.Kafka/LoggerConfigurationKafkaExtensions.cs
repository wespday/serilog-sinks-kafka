// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerConfigurationKafkaExtensions.cs" company="Wes Day">
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

namespace Serilog
{
    using System;
    using System.Diagnostics.Contracts;

    using Serilog.Configuration;
    using Serilog.Events;
    using Serilog.Sinks.Kafka;

    /// <summary>
    /// Logger configuration
    /// </summary>
    public static class LoggerConfigurationKafkaExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events to kafka
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="topic">The topic where the log will be written to.</param>
        /// <param name="kafkaHostPrimary">Kafka primary host.</param>
        /// <param name="kafkaHostSecondary">Kafka secondary host.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            string topic,
            string kafkaHostPrimary,
            string kafkaHostSecondary,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = 50,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            Contract.Requires(kafkaHostPrimary != null);
            Contract.Requires(kafkaHostSecondary != null);
            Contract.Assume(period.HasValue);

            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException("loggerConfiguration");
            }

            if (topic == null)
            {
                throw new ArgumentNullException("topic");
            }
            
            // var defaultedPeriod = period ?? MongoDBSink.DefaultPeriod;
            return loggerConfiguration.Sink(
                new KafkaSink(
                    new KafkaSinkOptions() { BatchPostingLimit = batchPostingLimit, Period = period.Value == null ? TimeSpan.FromMinutes(1) : period.Value },
                    kafkaHostPrimary,
                    kafkaHostSecondary,
                    topic));
        }
    }
}
