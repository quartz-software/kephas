﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionHelperTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="ReflectionHelper" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Test class for <see cref="ReflectionHelper"/>.
    /// </summary>
    [TestFixture]
    public class ReflectionHelperTest
    {
        [Test]
        public void GetNonGenericName_non_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(string));
            Assert.AreEqual("System.String", name);
        }

        [Test]
        public void GetNonGenericName_generic()
        {
            var name = ReflectionHelper.GetNonGenericFullName(typeof(IEnumerable<>));
            Assert.AreEqual("System.Collections.Generic.IEnumerable", name);
        }

        [Test]
        public void AsRuntimeAssemblyInfo()
        {
            var assemblyInfo = ReflectionHelper.AsRuntimeAssemblyInfo(this.GetType().Assembly);
            Assert.AreSame(assemblyInfo.GetUnderlyingAssemblyInfo(), this.GetType().Assembly);
        }

        [Test]
        public void GetPropertyName()
        {
            var propName = ReflectionHelper.GetPropertyName<ITypeInfo>(t => t.Name);
            Assert.AreEqual(nameof(ITypeInfo.Name), propName);
        }

        [Test]
        public void GetTypeInfo_non_IInstance()
        {
            var typeInfo = ReflectionHelper.GetTypeInfo("123");
            Assert.AreSame(typeof(string).AsRuntimeTypeInfo(), typeInfo);
        }

        [Test]
        public void GetTypeInfo_IInstance()
        {
            var instance = Substitute.For<IInstance>();
            var typeInfo = Substitute.For<ITypeInfo>();
            instance.GetTypeInfo().Returns(typeInfo);

            var obj = (object)instance;
            var objTypeInfo = ReflectionHelper.GetTypeInfo(obj);

            Assert.AreSame(typeInfo, objTypeInfo);
        }
    }
}