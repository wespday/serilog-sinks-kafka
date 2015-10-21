// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KafkaSink.cs" company="Wes Day">
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

namespace Serilog.Sinks.Kafka
{
    using System;
    using System.Diagnostics.Contracts;
    using PeriodicBatching;

    /// <summary>
    /// Writes log events as documents to Kafka.
    /// </summary>
    public class KafkaSink : PeriodicBatchingSink
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaSink"/> class.
        /// </summary>
        /// <param name="options">
        /// The configuration options.
        /// </param>
        public KafkaSink(KafkaSinkOptions options)
            : base(options.BatchPostingLimit, options.Period)
        {
            Contract.Requires<ArgumentNullException>(options != null);
            throw new NotImplementedException();
        }
    }
}
