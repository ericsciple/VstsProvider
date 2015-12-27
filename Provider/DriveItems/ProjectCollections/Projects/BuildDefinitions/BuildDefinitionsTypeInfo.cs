namespace VstsProvider.DriveItems.ProjectCollections.Projects.BuildDefinitions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class BuildDefinitionsTypeInfo : HttpClientContainerTypeInfo
    {
        public BuildDefinitionsTypeInfo()
        {
            this.AddChildTypeInfo(new BuildDefinitionTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "BuildDefinitions";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.BuildDefinitions.GetChildDriveItems(Segment)");
            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetDefinitionsAsync(
                            project: SegmentHelper.FindProjectName(segment))
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.Projects.Builds.GetChildDriveItems(Segment, Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return new[] {
                        this.ConvertToChildDriveItem(
                            segment,
                            httpClient
                            .GetDefinitionsAsync(
                                project: SegmentHelper.FindProjectName(segment),
                                name: childSegment.Name)
                            .Result
                            .Single())
                    };
                });
        }

        protected override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            return parentSegment
                .GetProvider()
                .PSVstsDriveInfo
                .GetHttpClient<BuildHttpClient>(
                    SegmentHelper.FindProjectCollectionName(parentSegment));
        }
    }
}
