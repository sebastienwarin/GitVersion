using System;
using System.Collections.Generic;
using System.Linq;
using GitVersion.VersionFilters;
using YamlDotNet.Serialization;

namespace GitVersion.Configuration
{
    public class IgnoreConfig
    {
        public IgnoreConfig()
        {
            SHAs = Enumerable.Empty<string>();
            PathFilters = new PathFilterConfig();
        }

        [YamlMember(Alias = "commits-before")]
        public DateTimeOffset? Before { get; set; }

        [YamlMember(Alias = "sha")]
        public IEnumerable<string> SHAs { get; set; }

        [YamlMember(Alias = "paths")]
        public PathFilterConfig PathFilters { get; set; }

        public virtual IEnumerable<IVersionFilter> ToFilters()
        {
            if (SHAs.Any()) yield return new ShaVersionFilter(SHAs);
            if (Before.HasValue) yield return new MinDateVersionFilter(Before.Value);
            foreach (var filter in PathFilters.ToFilters())
            {
                yield return filter;
            }
        }
    }
}
