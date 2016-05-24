// <copyright file="LoggerConfigurationKafkaExtensions.cs" company="Wes Day">
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

namespace Serilog
{
    using System;
    using System.Diagnostics.Contracts;

    using Serilog.Configuration;
    using Serilog.Sinks.Kafka;

    /// <summary>
    /// Logger configuration
    /// </summary>
    public static class LoggerConfigurationKafkaExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events to kafka
        /// </summary>
        /// <param name="loggerConfiguration">
        /// The logger configuration.
        /// </param>
        /// <param name="options">
        /// The <see cref="KafkaSinkOptions"/>.
        /// </param>
        /// <returns>
        /// Logger configuration, allowing configuration to continue.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// A required parameter is null.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller must dispose configuration")]
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            KafkaSinkOptions options)
        {
            Contract.Requires(loggerConfiguration != null);
            Contract.Requires(options != null);
            Contract.Ensures(Contract.Result<LoggerConfiguration>() != null);

            var result = loggerConfiguration.Sink(new KafkaSink(options));
            Contract.Assume(result != null);
            return result;
        }
    }
}
