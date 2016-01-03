namespace VstsProvider.DriveItems
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
            //this.provider.WriteDebug("DriveItems.Path.ctor(...)");
            //this.provider.WriteDebug(" this.rawPath: '{0}'", this.RawPath);
            //this.provider.WriteDebug(" this.provider.DriveInfo.Root: '{0}'", this.provider.PSVstsDriveInfo.Root);

            //string rootSegmentName;
            // string remainingPath =
            //     GetIsRooted(this.provider, this.RawPath)
            //     ? this.RawPath.Substring(startIndex: this.provider.PSVstsDriveInfo.Root.Length)
            //     : this.RawPath;

//             // Normalize and trim slashes.
//             Queue<string> remainingNames = new Queue<string>(
//                 remainingPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries));

            // Create the segments.
            this.rootSegment = new RootSegment(
                provider: this.provider,
                path: this,
                name: string.Concat(this.provider.PSVstsDriveInfo.Name, ":"),
                remainingNames: GetRemainingNames(this.provider, this.RawPath));
        }

        public string PSChildName
        {
            get
            {
                Segment lastSegment = this.Segments.Last();
                return lastSegment is RootSegment
                    ? string.Concat(this.rootSegment.Name, @"\")
                    : lastSegment.Name;
            }
        }

        public string PSParentPath
        {
            get
            {
                int totalCount = this.Segments.Count();
                switch (totalCount)
                {
                    case 1:
                        return string.Empty;
                    case 2:
                        return string.Concat(this.rootSegment.Name, @"\");
                    default:
                        return string.Join(@"\", this.Segments.Take(this.Segments.Count() - 1).Select(x => x.Name));
                }
            }
        }

        public string PSPath
        {
            get
            {
                int totalCount = this.Segments.Count();
                return totalCount == 1
                    ? string.Concat(this.rootSegment.Name, @"\")
                    : string.Join(@"\", this.Segments.Select(x => x.Name));
            }
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

        // public static string GetChildName(Provider provider, string rawPath)
        // {
        //     //provider.WriteDebug("VstsProvider.DriveItems.Path::GetChildName(...)");
        //     Path path = new Path(provider: provider, rawPath: rawPath);
        //     return path.Segments.Last() is RootSegment
        //         ? string.Empty
        //         : path.Segments.Last().Name;
        // }

        // public static string GetParentPathString(Provider provider, string rawPath)
        // {
        //     Path path = new Path(provider: provider, rawPath: rawPath);
        //     Segment lastSegment = path.Segments.Last();
        //     Segment lastParentSegment = lastSegment.GetParent();
        //     return lastSegment is RootSegment || lastParentSegment is RootSegment
        //         ? 
        //     {
        //     }
        //     }
        // }

        public IEnumerable<PSObject> GetItemByWildcard()
        {
            Segment lastSegment = this.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();
            return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).GetItemByWildcard(
                segment: lastParentSegment,
                childSegment: lastSegment);
        }

        public IEnumerable<PSObject> GetLiteralItem()
        {
            Segment lastSegment = this.Segments.Last();
            if (lastSegment is RootSegment)
            {
                // Return the drive.
                return new[]
                {
                    lastSegment.ItemTypeInfo.ConvertToDriveItem(
                        parentSegment: null,
                        obj: this.provider)
                };
            }

            // Query the child items of the last parent segment.
            Segment lastParentSegment = lastSegment.GetParent();
            return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo).GetLiteralItem(
                segment: lastParentSegment,
                childSegment: lastSegment);
        }

        public void ThrowInvalid(string prefixSentence)
        {
            throw new Exception(string.Format("{0} Invalid path: {1}", prefixSentence, this.RawPath));
        }

        // private static bool GetIsRooted(Provider provider, string rawPath)
        // {
        //     string normalizedRoot = string.Concat(provider.PSVstsDriveInfo.Root.Replace('/', '\\'), "\\");
        //     string normalizedPath = string.Concat((rawPath ?? string.Empty).Replace('/', '\\').TrimEnd('\\'), "\\");
        //     return normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
        // }
        private static Queue<string> GetRemainingNames(Provider provider, string rawPath)
        {
            string remainingPath;
            string drivePath = string.Concat(provider.PSVstsDriveInfo.Name, @":\");
            string normalizedPath = (rawPath ?? string.Empty).Replace('/', '\\');
            if (normalizedPath.StartsWith(drivePath, StringComparison.OrdinalIgnoreCase))
            {
                remainingPath = normalizedPath.Substring(drivePath.Length);
            }
            else
            {
                string normalizedRootPath = string.Concat(
                    provider.PSVstsDriveInfo.Root.Replace('/', '\\'),
                    @"\");
                if (!normalizedPath.StartsWith(normalizedRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException(string.Format("Unexpected path format. Unable to determine root for path: '{0}'.", rawPath));
                }

                remainingPath = normalizedPath.Substring(normalizedRootPath.Length);
            }

            // Split the remaining path into segment names and remove unnecessary slashes.
            return new Queue<string>(
                remainingPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
