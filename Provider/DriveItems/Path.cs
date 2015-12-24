namespace VsoProvider.DriveItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public sealed class Path
    {
        private readonly Provider provider;
        private readonly Segment rootSegment;

        public Path(Provider provider, string rawPath)
        {
            // Store the parameters.
            this.provider = provider;
            this.RawPath = rawPath;
            this.provider.WriteVerbose("DriveItems.Path.ctor(...)");
            this.provider.WriteVerbose("this.rawPath = {0}", this.RawPath);
            this.provider.WriteVerbose("this.provider.DriveInfo.Root = {0}", this.provider.PSVsoDriveInfo.Root);

            string rootSegmentName;
            string remainingSegmentNames;
            if (GetIsRooted(this.provider, this.RawPath))
            {
                // Extract the root segment name.
                int rootLength = this.provider.PSVsoDriveInfo.Root.TrimEnd('\\', '/').Length;
                rootSegmentName = this.RawPath.Substring(startIndex: 0, length: rootLength);

                // Extract the remaining segment names.
                remainingSegmentNames = this.RawPath.Substring(startIndex: rootLength);
            }
            else
            {
                // Set the root segment name.
                rootSegmentName = this.provider.PSVsoDriveInfo.Root.TrimEnd('\\', '/').Replace('/', '\\');

                // Set the remaining segment names and convert slashes.
                remainingSegmentNames = this.RawPath.Replace('/', '\\');
            }

            // Remove unneccesary slashes.
            remainingSegmentNames =
                remainingSegmentNames
                .Trim('\\')
                .Replace(@"\\", @"\");

            // Create the segments.
            this.rootSegment = new RootSegment(
                provider: this.provider,
                path: this,
                name: rootSegmentName,
                remainingNames: remainingSegmentNames);
        }

        public string RawPath { get; private set; }

        public IEnumerable<Segment> Segments
        {
            get
            {
                Segment segment = this.rootSegment;
                while (segment != null)
                {
                    yield return segment;
                    segment = segment.Child;
                }
            }
        }

        public static string GetChildName(Provider provider, string rawPath)
        {
            provider.WriteVerbose("VsoProvider.DriveItems.Path::GetChildName(...)");
            return new Path(provider: provider, rawPath: rawPath).Segments.Last().Name;
        }

        public IEnumerable<PSObject> GetChildDriveItems()
        {
            this.provider.WriteVerbose("DriveItems.Path.GetChildDriveItems()");
            Segment lastSegment = this.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();

            // Handle wildcard segment.
            if (lastSegment.HasWildcard)
            {
                this.provider.WriteVerbose("lastSegment.HasWildcard");
                return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).GetChildDriveItems(
                    segment: lastParentSegment,
                    childSegment: lastSegment);
            }

            // Handle container segment.
            if (lastSegment.ItemTypeInfo is ContainerTypeInfo)
            {
                return (lastSegment.ItemTypeInfo as ContainerTypeInfo)
                    .GetChildDriveItems(lastSegment);
            }

            // Handle leaf segment.
            PSObject obj =
            (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).GetChildDriveItems(
                segment: lastParentSegment,
                childSegment: lastSegment)
            .SingleOrDefault();
            if (obj == null)
            {
                this.ThrowInvalid(prefixSentence: "Item not found.");
            }

            return new[] { obj };
        }

        public IEnumerable<PSObject> GetDriveItem()
        {
            this.provider.WriteVerbose("VsoProvider.DriveItems.Path.GetDriveItem()");
            Segment lastSegment = this.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();
            if (lastParentSegment == null)
            {
                // Return the drive.
                yield return lastSegment.ItemTypeInfo.ConvertToDriveItem(parentSegment: null, obj: this.provider);
            }
            else
            {
                // Query the child items of the last parent segment.
                bool isFound = false;
                IEnumerable<PSObject> query = (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).GetChildDriveItems(
                    segment: lastParentSegment,
                    childSegment: lastSegment);
                foreach (PSObject psObject in query)
                {
                    isFound = true;
                    yield return psObject;
                }

                // Throw if not found and the last segment does not contain a wildcard.
                if (!isFound && !lastSegment.HasWildcard)
                {
                    this.ThrowInvalid(prefixSentence: "Item not found.");
                }
            }
        }

        public IEnumerable<PSObject> NewItem(object dynamicParameters)
        {
            Segment lastSegment = this.Segments.Last();
            if (lastSegment.HasWildcard)
            {
                this.ThrowInvalid("Item cannot contain wildcard characters.");
            }

            Segment lastParentSegment = lastSegment.GetParent();
            return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).NewChildDriveItem(
                segment: lastParentSegment,
                childSegment: lastSegment,
                dynamicParameters: dynamicParameters);
        }

        public object NewItemDynamicParameters()
        {
            Segment lastSegment = this.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();
            if (lastParentSegment == null)
            {
                return new object();
            }

            return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).NewChildDriveItemDynamicParameters();
        }

        public void ThrowInvalid(string prefixSentence)
        {
            throw new Exception(string.Format("{0} Invalid path: {1}", prefixSentence, this.RawPath));
        }

        private static bool GetIsRooted(Provider provider, string rawPath)
        {
            provider.WriteVerbose("VsoProvider.DriveItems.Path::GetIsRooted(...)");
            provider.WriteVerbose(string.Format("rawPath = {0}", rawPath));
            string normalizedRoot = provider.PSVsoDriveInfo.Root.Replace('/', '\\').TrimEnd('\\');
            provider.WriteVerbose(string.Format("normalizedRoot = {0}", normalizedRoot));
            return (rawPath ?? string.Empty).StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
        }
    }
}
