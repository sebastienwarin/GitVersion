using GitVersion.VersionCalculation.BaseVersionCalculators;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitVersion.VersionFilters
{
    public class PathFilter : IVersionFilter
    {
        public enum PathFilterMode { Inclusive, Exclusive }

        private readonly IEnumerable<string> paths;
        private readonly PathFilterMode mode;

        public PathFilter(IEnumerable<string> paths, PathFilterMode mode = PathFilterMode.Inclusive)
        {
            this.paths = paths ?? throw new ArgumentNullException(nameof(paths));
            this.mode = mode;
        }

        public bool Exclude(BaseVersion version, out string reason)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            return Exclude(version.BaseVersionSource, version.Context, out reason);
        }

        public bool Exclude(Commit commit, GitVersionContext context, out string reason)
        {
            if (commit == null) throw new ArgumentNullException(nameof(commit));

            reason = null;

            var match = new System.Text.RegularExpressions.Regex($"^({context.Configuration.GitTagPrefix})?.*$",
                System.Text.RegularExpressions.RegexOptions.Compiled);

            if (commit != null &&
                !context.Repository.Tags.Any(t => t.Target.Sha == commit.Sha && match.IsMatch(t.FriendlyName)))
            {
                Tree commitTree = commit.Tree; // Main Tree
                Tree parentCommitTree = commit.Parents.FirstOrDefault()?.Tree; // Secondary Tree
                Patch patch = context.Repository.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference
                switch (mode)
                {
                    case PathFilterMode.Inclusive:
                        if (!paths.Any(path => patch.Any(p => p.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase))))
                        {
                            reason = "Source was ignored due to commit path is not present";
                            return true;
                        }
                        break;
                    case PathFilterMode.Exclusive:
                        if (paths.Any(path => patch.All(p => p.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase))))
                        {
                            reason = "Source was ignored due to commit path excluded";
                            return true;
                        }
                        break;
                }
            }

            return false;
        }
    }
}
