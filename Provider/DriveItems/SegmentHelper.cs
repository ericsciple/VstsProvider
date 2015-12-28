namespace VstsProvider.DriveItems
{
    using System;
    using VstsProvider.DriveItems.Projects;
    using VstsProvider.DriveItems.Projects.Git;

    public static class SegmentHelper
    {
        public static string FindProjectCollectionName(Segment startingSegment)
        {
            return Find(startingSegment, "collection name", typeof(ProjectCollectionTypeInfo));
        }

        public static string FindProjectName(Segment startingSegment)
        {
            return Find(startingSegment, "team project name", typeof(ProjectTypeInfo));
        }

        public static string FindRepoName(Segment startingSegment)
        {
            return Find(startingSegment, "repo name", typeof(RepoTypeInfo));
        }

        private static string Find(Segment startingSegment, string description, params Type[] types)
        {
            while (startingSegment != null)
            {
                Type segmentItemTypeInfo = startingSegment.ItemTypeInfo.GetType();
                foreach (Type type in types)
                {
                    if (segmentItemTypeInfo == type)
                    {
                        return startingSegment.Name;
                    }
                }

                startingSegment = startingSegment.GetParent();
            }

            string message = string.Format("Unable to determine {0} from path.", description);
            throw new Exception(message);
        }
    }
}
