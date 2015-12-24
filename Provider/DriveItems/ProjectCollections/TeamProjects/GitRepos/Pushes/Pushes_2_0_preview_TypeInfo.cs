namespace VsoProvider.DriveItems.ProjectCollections.TeamProjects.GitRepos.Pushes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using System;
    public sealed class Pushes_2_0_preview_TypeInfo : WellKnownNameContainerTypeInfo
    {
        public Pushes_2_0_preview_TypeInfo()
        {
            this.AddChildTypeInfo(new Push_2_0_preview_TypeInfo());
        }

        public override string Name
        {
            get
            {
                return "Pushes_2.0-preview";
            }
        }

        public static PSObject AddBinaryFile(
            PSObject psObject,
            string refName,
            string oldObjectId,
            string comment,
            string pathWithinRepo,
            string literalPath)
        {
            // Load the file content.
            string content = Convert.ToBase64String(File.ReadAllBytes(literalPath), Base64FormattingOptions.None);

            // Add the binary file.
            return AddFile(
                psObject: psObject,
                refName: refName,
                oldObjectId: oldObjectId,
                comment: comment,
                pathWithinRepo: pathWithinRepo,
                content: content,
                contentType: "base64encoded");
        }

        public static PSObject AddTextFile(
            PSObject psObject,
            string refName,
            string oldObjectId,
            string comment,
            string pathWithinRepo,
            string literalPath,
            bool omitConvertNewLines)
        {
            // Load the file content.
            string content = File.ReadAllText(literalPath);
            if (!omitConvertNewLines)
            {
                content = content.Replace("\r\n", "\n");
            }

            // Add the text file.
            return AddTextFileFromContent(
                psObject: psObject,
                refName: refName,
                oldObjectId: oldObjectId,
                comment: comment,
                pathWithinRepo: pathWithinRepo,
                content: content);
        }

        public static PSObject AddTextFileFromContent(
            PSObject psObject,
            string refName,
            string oldObjectId,
            string comment,
            string pathWithinRepo,
            string content)
        {
            return AddFile(
                psObject: psObject,
                refName: refName,
                oldObjectId: oldObjectId,
                comment: comment,
                pathWithinRepo: pathWithinRepo,
                content: content,
                contentType: "rawtext");
        }

        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.Methods.Add(new PSCodeMethod("AddBinaryFile", this.GetType().GetMethod("AddBinaryFile", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("AddTextFile", this.GetType().GetMethod("AddTextFile", BindingFlags.Public | BindingFlags.Static)));
            psObject.Methods.Add(new PSCodeMethod("AddTextFileFromContent", this.GetType().GetMethod("AddTextFileFromContent", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/pushes?api-version=2.0-preview&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment));
        }

        public override IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            if (childSegment.HasWildcard)
            {
                return base.GetChildDriveItems(segment, childSegment);
            }

            return this.InvokeGetWebRequest(
                segment,
                "{0}/_apis/git/repositories/{2}/pushes/{3}?api-version=2.0-preview&projectId={1}",
                SegmentHelper.FindProjectCollectionName(segment),
                SegmentHelper.FindTeamProjectName(segment),
                SegmentHelper.FindRepoName(segment),
                childSegment.Name);
        }

        private static PSObject AddFile(
            PSObject psObject,
            string refName,
            string oldObjectId,
            string comment,
            string pathWithinRepo,
            string content,
            string contentType)
        {
            // Format the relative URL.
            Pushes_2_0_preview_TypeInfo typeInfo = psObject.GetPSVsoTypeInfo() as Pushes_2_0_preview_TypeInfo;
            Segment parentSegment = psObject.GetPSVsoParentSegment();
            //// The pushes 2.0 controller ignores the project filter.
            //// Therefore resolve the repo ID in a hacky way.
            DriveItems.Path repoPath = new DriveItems.Path(
                provider: psObject.GetPSVsoProvider(),
                rawPath: parentSegment.RelativePath);
            string repoId = repoPath.GetDriveItem().Single().Properties["id"].Value as string;
            string relativeUrl = typeInfo.UrlStringFormat(
                "{0}/_apis/git/repositories/{1}/pushes?api-version=2.0-preview",
                SegmentHelper.FindProjectCollectionName(parentSegment),
                repoId);

            // Format the body JSON.
            const string BodyJsonFormat = @"{{
    ""refUpdates"": [
        {{
            ""name"": ""{0}"",
            ""oldObjectId"": ""{1}""
        }}
    ],
    ""commits"": [
        {{
            ""comment"": ""{2}"",
            ""changes"": [
                {{
                    ""changeType"": ""add"",
                    ""item"": {{
                        ""path"": ""{3}""
                    }},
                    ""newContent"": {{
                        ""content"": ""{4}"",
                        ""contentType"": ""{5}""
                    }}
                }}
            ]
        }}
    ]
}}";
            string bodyJson = typeInfo.JsonStringFormat(
                BodyJsonFormat,
                refName,
                oldObjectId,
                comment,
                pathWithinRepo,
                content,
                contentType);

            // POST the HTTP web request.
            return typeInfo.InvokePostWebRequest(
                    provider: parentSegment.GetProvider(),
                    relativeUrl: relativeUrl,
                    bodyJson: bodyJson)
                .SingleOrDefault();
        }
    }
}
