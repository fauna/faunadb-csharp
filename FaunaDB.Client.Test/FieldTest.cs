﻿using System;
using System.Collections.Generic;
using FaunaDB.Collections;
using FaunaDB.Types;
using NUnit.Framework;
using static FaunaDB.Types.Option;

namespace Test
{
    [TestFixture]
    public class FieldTest
    {
        [Test]
        public void TestObjectKey()
        {
            var obj = ObjectV.With("foo", "bar");

            Assert.AreEqual(StringV.Of("bar"),
                obj.Get(Field.At("foo")));

            Assert.AreEqual(None(),
                obj.GetOption(Field.At("nonexistent")));

            Assert.Throws(typeof(InvalidOperationException),
                () => obj.Get(Field.At("nonexistent")),
                "Cannot find path \"nonexistent\". Object key \"nonexistent\" not found");
        }

        [Test]
        public void TestArrayIndex()
        {
            var array = ArrayV.Of("a string", 10);

            Assert.AreEqual(StringV.Of("a string"),
                array.Get(Field.At(0)));

            Assert.AreEqual(LongV.Of(10),
                array.Get(Field.At(1)));

            Assert.AreEqual(None(),
                array.GetOption(Field.At(1234)));

            Assert.Throws(typeof(InvalidOperationException),
                () => array.Get(Field.At(1234)),
                "Cannot find path \"1234\". Array index \"1234\" not found");
        }

        [Test]
        public void TestNestedObject()
        {
            var nested = ObjectV.With("foo", ObjectV.With("bar", "a string"));

            Assert.AreEqual(StringV.Of("a string"),
                nested.Get(Field.At("foo", "bar")));

            Assert.Throws(typeof(InvalidOperationException),
                () => nested.Get(Field.At("foo", "nonexistent")),
                "Cannot find path \"foo/nonexistent\". Object key \"nonexistent\" not found");
        }

        [Test]
        public void TestNestedArray()
        {
            var nested = ArrayV.Of(10, ArrayV.Of(1234, ArrayV.Of(4321)));

            Assert.AreEqual(LongV.Of(10),
                nested.Get(Field.At(0)));

            Assert.AreEqual(LongV.Of(1234),
                nested.Get(Field.At(1, 0)));

            Assert.AreEqual(LongV.Of(4321),
                nested.Get(Field.At(1, 1, 0)));

            Assert.Throws(typeof(InvalidOperationException),
                () => nested.Get(Field.At(1, 1, 1)),
                "Cannot find path \"1/1/1\". Array index \"1\" not found");
        }

        [Test]
        public void TestFieldConvertion()
        {
            var setRef = new Dictionary<string, Value>
            {
                {"@ref", "databases" },
            };

            var obj = ObjectV.With(
                "string", "a string",
                "bool", true,
                "double", 3.14,
                "long", 1234,
                "ref", Native.DATABASES,
                "setref", new SetRefV(setRef));

            Assert.AreEqual("a string",
                obj.Get(Field.At("string").To<string>()));

            Assert.AreEqual(true,
                obj.Get(Field.At("bool").To<bool>()));

            Assert.AreEqual(3.14,
                obj.Get(Field.At("double").To<double>()));

            Assert.AreEqual(1234L,
                obj.Get(Field.At("long").To<long>()));

            Assert.AreEqual(Native.DATABASES,
                obj.Get(Field.At("ref").To<RefV>()));

            Assert.AreEqual(new SetRefV(setRef),
                obj.Get(Field.At("setref").To<SetRefV>()));
        }

        [Test]
        public void TestComplex()
        {
            var obj = ObjectV.With("foo", ArrayV.Of(1, 2, ObjectV.With("bar", "a string")));

            Assert.AreEqual("a string",
                obj.Get(Field.At("foo").At(Field.At(2)).At(Field.At("bar").To<string>())));
        }

        [Test]
        public void TestCollect()
        {
            var array = ArrayV.Of("John", "Bill");

            Assert.That(array.Collect(Field.To<string>()),
                        Is.EquivalentTo(new List<string> { "John", "Bill" }));

            var obj = ObjectV.With("arrayOfNames", array);

            Assert.That(obj.Get(Field.At("arrayOfNames").Collect(Field.To<string>())),
                        Is.EquivalentTo(new List<string> { "John", "Bill" }));

            Assert.Throws(typeof(InvalidOperationException),
                () => obj.Collect(Field.To<string>()),
                "Cannot convert ObjectV to ArrayV");
        }

        [Test]
        public void TestCollectComplex()
        {
            var array = ArrayV.Of(
                    ObjectV.With("name", ArrayV.Of("John")),
                    ObjectV.With("name", ArrayV.Of("Bill"))
                );

            Assert.That(array.Collect(Field.At("name").At(Field.At(0)).To<string>()),
                        Is.EquivalentTo(new List<string> { "John", "Bill", }));
        }
    }
}
