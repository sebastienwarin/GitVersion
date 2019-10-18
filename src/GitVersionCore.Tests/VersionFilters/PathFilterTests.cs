using System;
using GitVersion;
using GitVersion.VersionCalculation.BaseVersionCalculators;
using GitVersion.VersionFilters;
using NUnit.Framework;
using Shouldly;
using GitVersionCore.Tests.Mocks;

namespace GitVersionCore.Tests.VersionFilters
{
    [TestFixture]
    public class PathFilterTests : TestBase
    {
        [Test]
        public void VerifyNullGuard()
        {
            Should.Throw<ArgumentNullException>(() => new PathFilter(null));
        }

        [Test]
        public void VerifyNullGuard2()
        {
            var sut = new PathFilter(new[] { "" });

            Should.Throw<ArgumentNullException>(() => sut.Exclude(null, out _));
        }
    }
}
