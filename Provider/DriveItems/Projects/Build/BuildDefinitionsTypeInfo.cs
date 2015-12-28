namespace VstsProvider.DriveItems.Projects.Build
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class BuildDefinitionsTypeInfo : BuildHttpClientContainerTypeInfo
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
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.BuildDefinitions.GetChildDriveItems(Segment)");
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
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.BuildDefinitions.GetChildDriveItems(Segment, Segment)");
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
    }
}
