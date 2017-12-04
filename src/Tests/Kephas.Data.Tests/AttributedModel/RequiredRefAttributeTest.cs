﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiredRefAttributeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the required reference attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.AttributedModel
{
    using System;

    using Kephas.Data.AttributedModel;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class RequiredRefAttributeTest
    {
        [Test]
        public void Validate_value()
        {
            var attr = new RequiredRefAttribute();
            Assert.IsFalse(attr.IsValid(0));
            Assert.IsTrue(attr.IsValid(-1));
            Assert.IsFalse(attr.IsValid(null));
            Assert.IsFalse(attr.IsValid(Guid.Empty));
        }

        [Test]
        public void Validate_ref()
        {
            var r = Substitute.For<IRef>();
            var attr = new RequiredRefAttribute();

            r.IsEmpty.Returns(true);
            Assert.IsFalse(attr.IsValid(r));

            r.IsEmpty.Returns(false);
            Assert.IsTrue(attr.IsValid(r));
        }
    }
}