﻿#nullable enable

using FluentAssertions;
using Hl7.Cql.Primitives;
using Hl7.Cql.ValueSetLoaders;
using Hl7.Cql.ValueSets;
using Hl7.Fhir.Specification.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreTests
{

    [TestClass]
    public class ValueSetTests
    {
        private static readonly CqlCode[] TestCodesA = new[] {
            new CqlCode("a", "http://nu.nl"),
            new CqlCode("b", "http://nu.nl"),
            new CqlCode("c", "HTTP://nu.nl"),
            new CqlCode("d", null) };

        private static readonly CqlCode[] TestCodesB = new[] {
            new CqlCode("e", "http://nu.nl", display: "Letters"),
            new CqlCode("f", "http://nu.nl", display: "LETTERS"),
            new CqlCode("g", null, display: "Letters"),
            new CqlCode("h", "http://nu.nl"),
            new CqlCode("h", "http://nu.nl", display: "Letters"),
        };


        [TestMethod]
        public void CanReturnAllCodeInValueSet()
        {
            var dict = buildDict();
            dict.TryGetCodesInValueSet("http://valuesetA", out var vsA).Should().BeTrue();
            allCodesInA(vsA!);

            dict.TryGetCodesInValueSet("http://valuesetB", out var vsB).Should().BeTrue();
            allCodesInB(vsB!);
        }

        private void allCodesInA(IEnumerable<CqlCode> vs)
        {
            TestCodesA.All(c => vs.Contains(c)).Should().BeTrue();
            vs.All(c => TestCodesA.Contains(c)).Should().BeTrue();
        }

        private void allCodesInB(IEnumerable<CqlCode> vs)
        {
            TestCodesB.All(c => vs.Contains(c)).Should().BeTrue();
            vs.All(c => TestCodesB.Contains(c)).Should().BeTrue();
        }

        [TestMethod]
        public void CanGetFacade()
        {
            var dict = buildDict();
            var f = dict.GetValueSet("http://valuesetA");
            allCodesInA(f!);
        }

        private InMemoryValueSetDictionary buildDict()
        {
            var dict = new InMemoryValueSetDictionary();

            dict.Add("http://valuesetA", TestCodesA);
            dict.Add("http://valuesetB", TestCodesB);

            return dict;
        }

        [TestMethod]
        public void HashDictonaryInternsCodes()
        {
            var dict = new InMemoryValueSetDictionary();

            dict.Add("http://valuesetA", new[] { new CqlCode("x", "http://nu.nl", "1.0"), new CqlCode("y", "http://nu.nl") });
            dict.Add("http://valuesetB", new[] { new CqlCode("x", "http://nu.nl"), new CqlCode("y", "http://nu.nl") });

            _ = dict.TryGetCodesInValueSet("http://valuesetA", out var vsA);
            _ = dict.TryGetCodesInValueSet("http://valuesetB", out var vsB);

            ReferenceEquals(vsA!.Single(c => c.code == "y"), vsB!.Single(c => c.code == "y")).Should().BeTrue();
            ReferenceEquals(vsA!.Single(c => c.code == "x"), vsB!.Single(c => c.code == "x")).Should().BeFalse();
        }

        [TestMethod]
        public void Intensional_Value_Set()
        {
            var resolver = new DirectorySource(@"Input\ValueSets");
            var vsd = new ValueSetSource(resolver);

            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/2.16.840.1.113883.3.464.1004.1009", out var codes1009));
            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/2.16.840.1.113883.3.464.1004.1013", out var codes1013));
            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/intensional-value-set", out var intensional));

            // Note that this tests uses *reference equality* to test membership, which only works because we're storing
            // all codes into the HashValueSetDictionary, which interns all codes.
            Assert.IsTrue(codes1009!.All(c => intensional!.Contains(c)));
            Assert.IsTrue(codes1013!.All(c => intensional!.Contains(c)));
        }

        [TestMethod]
        public void Intensional_Value_Set_2_Levels()
        {
            var resolver = new DirectorySource(@"Input\ValueSets");
            var vsd = new ValueSetSource(resolver);

            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/2.16.840.1.113883.3.464.1004.1009", out var codes1009));
            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/2.16.840.1.113883.3.464.1004.1013", out var codes1013));
            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/intensional-value-set", out var intensional));

            // Note that this tests uses *reference equality* to test membership, which only works because we're storing
            // all codes into the HashValueSetDictionary, which interns all codes.
            Assert.IsTrue(codes1009!.All(c => intensional!.Contains(c)));
            Assert.IsTrue(codes1013!.All(c => intensional!.Contains(c)));

            Assert.IsTrue(vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/intensional-value-set-2", out var intensional2));
            Assert.IsTrue(codes1009!.All(c => intensional2!.Contains(c)));
            Assert.IsTrue(codes1013!.All(c => intensional2!.Contains(c)));

        }

        [TestMethod, Ignore("Until we have SDK 5.4, which will detect cycles.")]
        public void Intensional_Value_Set_Cycle()
        {
            var resolver = new DirectorySource(@"Input\ValueSets");
            var vsd = new ValueSetSource(resolver);

            Assert.ThrowsException<InvalidOperationException>(() => vsd.TryGetCodesInValueSet("https://www.ncqa.org/fhir/valueset/intensional-value-set-3", out var _));
        }
    }
}

#nullable restore