using System;
using System.IO;
using System.Text;
using GitVersion;
using GitVersion.OutputVariables;
using NUnit.Framework;
using Shouldly;
using GitVersion.Extensions;
using GitVersion.Logging;

namespace GitVersionCore.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    class WixFileTests
    {
        [SetUp]
        public void Setup()
        {
            ShouldlyConfiguration.ShouldMatchApprovedDefaults.LocateTestMethodUsingAttribute<TestAttribute>();
        }

        [Test]
        [Category("NoMono")]
        [Description("Won't run on Mono due to source information not being available for ShouldMatchApproved.")]
        public void UpdateWixVersionFile()
        {
            var fileSystem = new TestFileSystem();
            var workingDir = Path.GetTempPath();
            var semVer = new SemanticVersion
            {
                Major = 1,
                Minor = 2,
                Patch = 3,
                BuildMetaData = "5.Branch.develop"
            };

            semVer.BuildMetaData.VersionSourceSha = "versionSourceSha";
            semVer.BuildMetaData.Sha = "commitSha";
            semVer.BuildMetaData.ShortSha = "commitShortSha";
            semVer.BuildMetaData.CommitDate = DateTimeOffset.Parse("2019-02-20 23:59:59Z");

            var config = new TestEffectiveConfiguration(buildMetaDataPadding: 2, legacySemVerPadding: 5);

            var variableProvider = new VariableProvider(new NullLog());
            var vars = variableProvider.GetVariablesFor(semVer, config, false);

            var stringBuilder = new StringBuilder();
            void Action(string s) => stringBuilder.AppendLine(s);

            var logAppender = new TestLogAppender(Action);
            var log = new Log(logAppender);

            using (var wixVersionFileUpdater = new WixVersionFileUpdater(workingDir, vars, fileSystem, log))
            {
                wixVersionFileUpdater.Update();
                fileSystem.ReadAllText(wixVersionFileUpdater.WixVersionFile).
                    ShouldMatchApproved(c => c.SubFolder(Path.Combine("Approved")));
            }
        }
    }
}
