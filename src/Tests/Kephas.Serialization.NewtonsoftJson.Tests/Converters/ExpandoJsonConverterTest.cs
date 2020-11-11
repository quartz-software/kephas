﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoJsonConverterTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json.Tests.Converters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using NUnit.Framework;

    [TestFixture]
    public class ExpandoJsonConverterTest : SerializationTestBase
    {
        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Expando
            {
                ["Description"] = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""Description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_IExpando()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""Description"":""John Doe""}", GetSerializationContext(typeof(IExpando)));

            Assert.IsInstanceOf<Expando>(obj);
            var expando = (Expando)obj;
            Assert.AreEqual("John Doe", expando["Description"]);
        }

        [Test]
        public async Task SerializeAsync_ExpandoEntity()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
            {
                Description = "John Doe",
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_ExpandoEntity()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}");

            Assert.IsInstanceOf<ExpandoEntity>(obj);
            var expando = (ExpandoEntity)obj;
            Assert.AreEqual("John Doe", expando.Description);
        }

        [Test]
        public async Task SerializeAsync_Wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new Wrapper { Value = new ExpandoEntity { Description = "John Doe" } };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+Wrapper"",""value"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_Wrapper()
        {
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(@"{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+Wrapper"",""value"":{""$type"":""Kephas.Serialization.Json.Tests.Converters.ExpandoJsonConverterTest+ExpandoEntity"",""description"":""John Doe""}}");

            Assert.IsInstanceOf<Wrapper>(obj);
            var wrapper = (Wrapper)obj;

            Assert.IsInstanceOf<ExpandoEntity>(wrapper.Value);
            var expando = (ExpandoEntity)wrapper.Value;
            Assert.AreEqual("John Doe", expando.Description);
        }

        [Test]
        public async Task DeserializeAsync_Typeless_Expando()
        {
            var typelessJson = @"
{
  ""connection-strategy"": {
    ""match"": {
            ""script-type"": {
                ""sh"": ""DemoTasks/ConnectionStrategies/Shell/ProxyConnection""
            }
        },
    ""override"": {
      ""gateway"": {
        ""ref"": ""Systems-with-gateway-wsdev1/wsdev1""
      }
    },
    ""connection-flow"": [
      {
        ""name"": ""one""
      },
      {
        ""name"": ""two""
      }
    ]
  }
}";
            var settingsProvider = GetJsonSerializerSettingsProvider();
            var serializer = new JsonSerializer(settingsProvider);
            var obj = await serializer.DeserializeAsync(typelessJson, this.GetSerializationContext(typeof(IExpando)));

            Assert.IsInstanceOf<Expando>(obj);
            var expando = (Expando)obj;
            var strategy = (IIndexable)expando["connection-strategy"];
            Assert.IsNotNull(strategy);
            var connectionFlow = (IList<object?>)strategy["connection-flow"];
            Assert.AreEqual(2, connectionFlow.Count);
        }

        public class ExpandoEntity : Expando
        {
            public string Description { get; set; }
        }

        public class Wrapper
        {
            public IExpando Value { get; set; }
        }
    }
}