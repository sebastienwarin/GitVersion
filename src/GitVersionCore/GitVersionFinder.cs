using System.IO;
using GitVersion.Exceptions;
using GitVersion.VersionCalculation;
using GitVersion.Helpers;
using GitVersion.Logging;

namespace GitVersion
{
    public class GitVersionFinder
    {
        public SemanticVersion FindVersion(ILog log, GitVersionContext context)
        {
            log.Info($"Running against branch: {context.CurrentBranch.FriendlyName} ({(context.CurrentCommit == null ? "-" : context.CurrentCommit.Sha)})");
            if (context.IsCurrentCommitTagged)
            {
                log.Info($"Current commit is tagged with version {context.CurrentCommitTaggedVersion}, " +
                                 "version calculation is for metadata only.");
            }
            EnsureMainTopologyConstraints(context);

            var filePath = Path.Combine(context.Repository.GetRepositoryDirectory(), "NextVersion.txt");
            if (File.Exists(filePath))
            {            
                throw new WarningException("NextVersion.txt has been deprecated. See http://gitversion.readthedocs.org/en/latest/configuration/ for replacement");
            }

            return new NextVersionCalculator(log).FindVersion(context);
        }

        void EnsureMainTopologyConstraints(GitVersionContext context)
        {
            EnsureHeadIsNotDetached(context);
        }

        void EnsureHeadIsNotDetached(GitVersionContext context)
        {
            if (!context.CurrentBranch.IsDetachedHead())
            {
                return;
            }

            var message = string.Format(
                "It looks like the branch being examined is a detached Head pointing to commit '{0}'. " +
                "Without a proper branch name GitVersion cannot determine the build version.",
                context.CurrentCommit.Id.ToString(7));
            throw new WarningException(message);
        }
    }
}
