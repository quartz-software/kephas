﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginData.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugin data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Application;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A plugin data.
    /// </summary>
    public sealed class PluginData
    {
        private const int MissingPartsInvalidCode = 1;
        private const int ParseStateInvalidCode = 2;
        private const int ParseChecksumInvalidCode = 3;
        private const int ChecksumInvalidCode = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginData"/> class.
        /// </summary>
        /// <param name="identity">The plugin identity.</param>
        /// <param name="state">The plugin state.</param>
        /// <param name="data">Optional. The additional data associated with the plugin.</param>
        public PluginData(AppIdentity identity, PluginState state, IDictionary<string, string> data = null)
        {
            Requires.NotNull(identity, nameof(identity));

            this.Identity = identity;
            this.State = state;
            this.Data = data ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the identifier of the plugin.
        /// </summary>
        /// <value>
        /// The identifier of the plugin.
        /// </value>
        public AppIdentity Identity { get; }

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <value>
        /// The plugin state.
        /// </value>
        public PluginState State { get; }

        /// <summary>
        /// Gets additional data associated with the license.
        /// </summary>
        /// <value>
        /// The additional data associated with the license.
        /// </value>
        public IDictionary<string, string> Data { get; }

        /// <summary>
        /// Parses the value and returns valid plugin data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A PluginData.
        /// </returns>
        public static PluginData Parse(string value)
        {
            Requires.NotNull(value, nameof(value));

            var splits = value.Split('\n');
            var appId = AppIdentity.Parse(splits[0]);

            if (splits.Length < 3)
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({MissingPartsInvalidCode}).");
            }

            if (!Enum.TryParse<PluginState>(splits[1], out var state))
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseStateInvalidCode}).");
            }

            var data = splits.Length > 3 ? DataParse(splits.Skip(2).Take(splits.Length - 3)) : null;

            if (!int.TryParse(splits[splits.Length - 1], out var checksum))
            {
                throw new InvalidPluginDataException($"The plugin data for {appId} is corrupt, probably was manually changed ({ParseChecksumInvalidCode}).");
            }

            var pluginData = new PluginData(appId, state, data);
            pluginData.Validate(checksum);

            return pluginData;
        }

        /// <summary>
        /// Creates a clone of the plugin data instance with the provided changed state.
        /// </summary>
        /// <param name="state">The plugin state.</param>
        /// <returns>
        /// A new <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeState(PluginState state)
        {
            return new PluginData(this.Identity, state, this.Data.ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// Creates a clone of the plugin data instance with the provided changed data entry.
        /// </summary>
        /// <param name="key">The data entry key.</param>
        /// <param name="value">The data entry value.</param>
        /// <returns>
        /// A new <see cref="PluginData"/>.
        /// </returns>
        public PluginData ChangeData(string key, string value)
        {
            var data = this.Data.ToDictionary(kv => kv.Key, kv => kv.Value);
            data[key] = value;
            return new PluginData(this.Identity, this.State, data);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Identity}\n{this.State}\n{DataToString(this.Data)}\n{this.GetChecksum()}";
        }

        private static IDictionary<string, string> DataParse(IEnumerable<string> values)
        {
            var data = new Dictionary<string, string>();
            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var pos = value.IndexOf(':');
                if (pos >= 0)
                {
                    data[value.Substring(0, pos)] = value.Substring(pos + 1);
                }
                else
                {
                    data[value] = null;
                }
            }

            return data;
        }

        private static string DataToString(IDictionary<string, string> data)
        {
            if (data == null || data.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("\n", data.Select(kv => $"{kv.Key}:{kv.Value}"));
        }

        private void Validate(int checksum)
        {
            if (this.GetChecksum() == checksum)
            {
                return;
            }

            throw new InvalidPluginDataException($"The plugin data for {this.Identity} is corrupt, probably was manually changed ({ChecksumInvalidCode}).");
        }

        private int GetChecksum()
        {
            var identityChecksum = this.GetChecksum(this.Identity.ToString());
            var stateChecksum = this.GetChecksum(this.State.ToString());
            var dataChecksum = this.GetChecksum(DataToString(this.Data));

            unchecked
            {
                return identityChecksum
                    + (stateChecksum << 1)
                    + (dataChecksum << 2);
            }
        }

        private int GetChecksum(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
