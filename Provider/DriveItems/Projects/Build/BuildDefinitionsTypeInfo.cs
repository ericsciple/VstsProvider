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

        public override IEnumerable<PSObject> GetItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.BuildDefinitions.GetItems(Segment)");
            BuildHttpClient httpClient = this.GetHttpClient(segment) as BuildHttpClient;
            return this.Wrap(
                segment,
                () =>
                {
                    return httpClient
                        .GetDefinitionsAsync(
                            project: SegmentHelper.GetProjectName(segment))
                        .Result
                        .Select(x => this.ConvertToChildDriveItem(segment, x))
                        .ToArray();
                });
        }

        public override IEnumerable<PSObject> GetLiteralItem(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.Projects.Build.BuildDefinitions.GetLiteralItem(Segment, Segment)");
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
                                project: SegmentHelper.GetProjectName(segment),
                                name: childSegment.UnescapedName)
                            .Result
                            .Single())
                    };
                });
        }
    }
}
