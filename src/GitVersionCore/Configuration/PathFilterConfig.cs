using GitVersion.VersionFilters;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace GitVersion.Configuration
{
    public class PathFilterConfig
    {
        public PathFilterConfig()
        {
            Include = Enumerable.Empty<string>();
            Exclude = Enumerable.Empty<string>();
        }
        
        [YamlMember(Alias = "exclude")]
        public IEnumerable<string> Exclude { get; set; }

        [YamlMember(Alias = "include")]
        public IEnumerable<string> Include { get; set; }

        public virtual IEnumerable<IVersionFilter> ToFilters()
        {
            if (Include.Any()) yield return new PathFilter(Include, PathFilter.PathFilterMode.Inclusive);
            if (Exclude.Any()) yield return new PathFilter(Exclude, PathFilter.PathFilterMode.Exclusive);
        }
    }
}
