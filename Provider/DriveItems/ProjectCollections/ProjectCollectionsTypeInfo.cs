﻿namespace VstsProvider.DriveItems.ProjectCollections
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    public sealed class ProjectCollectionsTypeInfo : HttpClientContainerTypeInfo
    {
        public ProjectCollectionsTypeInfo()
        {
            this.AddChildTypeInfo(new ProjectCollectionTypeInfo());
        }

        public override string Name
        {
            get
            {
                return "ProjectCollections";
            }
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment)");
            ProjectCollectionHttpClient httpClient = this.GetHttpClient(segment) as ProjectCollectionHttpClient;
            return this.Wrap((int? top, int? skip) =>
            {
                return httpClient
                    .GetProjectCollections(
                        top: top,
                        skip: skip,
                        userState: null)
                    .Result
                    .Select(x => this.ConvertToChildDriveItem(segment, x))
                    .ToArray();
            });
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            segment.GetProvider().WriteDebug("DriveItems.ProjectCollections.GetChildDriveItems(Segment,Segment)");
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            ProjectCollectionHttpClient httpClient = this.GetHttpClient(segment) as ProjectCollectionHttpClient;
            return this.Wrap(() =>
            {
                return new[] {
                    this.ConvertToChildDriveItem(
                        segment,
                        httpClient
                        .GetProjectCollection(
                            id: childSegment.Name,
                            userState: null)
                        .Result)
                };
            });
        }

        protected override VssHttpClientBase GetHttpClient(Segment parentSegment)
        {
            return parentSegment
                .GetProvider()
                .PSVstsDriveInfo
                .GetHttpClient<ProjectCollectionHttpClient>();
        }
    }
}
