namespace VstsProvider.DriveItems
{
    using System;
    using VstsProvider.DriveItems.Projects;
    using VstsProvider.DriveItems.Projects.Git;

    public static class SegmentHelper
    {
        public static string Escape(string name)
        {
            return (name ?? string.Empty)
                .Replace("%", "%25") // Must be escaped first.
                .Replace(@"\", "%5C")
                .Replace("/", "%2F");
        }

        public static string GetBranchName(Segment startingSegment)
        {
            string refName = GetUnescaped(startingSegment, "ref name", typeof(RefTypeInfo));
            if (!refName.StartsWith("heads/"))
            {
                throw new Exception(string.Format("Unexpected branch name format: {0}. Expected format: heads/[...]", refName));
            }

            return refName.Substring("heads/".Length);
        }

        public static string GetHttpClientProjectCollectionName(string root, Segment startingSegment)
        {
            string upperHost = new Uri(root).Host.ToUpperInvariant();
            if (upperHost.EndsWith(".VISUALSTUDIO.COM") ||
                upperHost.EndsWith(".TFSALLIN.NET"))
            {
                return "DefaultCollection";
            }

            return GetUnescaped(startingSegment, "collection name", typeof(ProjectCollectionTypeInfo));
        }

        public static string GetProjectName(Segment startingSegment)
        {
            return GetUnescaped(startingSegment, "team project name", typeof(ProjectTypeInfo));
        }

        public static string GetRepoName(Segment startingSegment)
        {
            return GetUnescaped(startingSegment, "repo name", typeof(RepoTypeInfo));
        }
        
        public static string Unescape(string name)
        {
            return (name ?? string.Empty)
                .Replace("%5C", @"\")
                .Replace("%2F", "/")
                .Replace("%25", "%"); // Must be unescaped last.
        }

        private static string GetUnescaped(Segment startingSegment, string description, params Type[] types)
        {
            while (startingSegment != null)
            {
                Type segmentItemTypeInfo = startingSegment.ItemTypeInfo.GetType();
                foreach (Type type in types)
                {
                    if (segmentItemTypeInfo == type)
                    {
                        return Unescape(startingSegment.Name);
                    }
                }

                startingSegment = startingSegment.GetParent();
            }

            string message = string.Format("Unable to determine {0} from path.", description);
            throw new Exception(message);
        }
    }
}
