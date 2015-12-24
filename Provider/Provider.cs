namespace VsoProvider
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Provider;
    using VsoProvider.DriveItems;

    [CmdletProvider("VSO", ProviderCapabilities.ExpandWildcards)]
    public sealed class Provider : NavigationCmdletProvider
    {
        public DriveInfo PSVsoDriveInfo
        {
            get
            {
                return this.PSDriveInfo as DriveInfo;
            }
        }

        public void WriteVerbose(string format, params object[] args)
        {
            string message =
                args.Length == 0
                ? format
                : string.Format(CultureInfo.InvariantCulture, format, args);
            base.WriteVerbose(message);
        }

        protected override string[] ExpandPath(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.ExpandPath(...)");
            Path path = this.ParsePath(rawPath);
            Segment lastSegment = path.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();
            WildcardPattern pattern = new WildcardPattern(lastSegment.Name, WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);

            // TODO: IS THIS REQUIRED?
            if (lastParentSegment.ItemTypeInfo is LeafTypeInfo)
            {
                return new string[0];
            }

            return (lastParentSegment.ItemTypeInfo as ContainerTypeInfo)
                .GetChildDriveItems(lastParentSegment)
                .Select(x => x.GetPSVsoName())
                .Where(x => pattern.IsMatch(x))
                .Select(x => string.Format(@"{0}\{1}", lastParentSegment.RelativePath, x))
                .ToArray();
        }

        protected override void GetChildItems(string rawPath, bool recurse)
        {
            this.WriteVerbose("VsoProvider.Provider.GetChildItems(...)");
            Path path = this.ParsePath(rawPath);
            Segment lastSegment = path.Segments.Last();
            Segment lastContainerSegment =
                lastSegment.ItemTypeInfo is ContainerTypeInfo
                ? lastSegment
                : lastSegment.GetParent();
            foreach (PSObject psObject in path.GetChildDriveItems())
            {
                string relativePath =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        @"{0}\{1}",
                        lastContainerSegment.RelativePath,
                        psObject.GetPSVsoName())
                    .TrimStart('\\');
                this.WriteItemObject(
                    item: psObject,
                    path: relativePath,
                    isContainer: psObject.GetPSVsoIsContainer());
            }

            if (recurse)
            {
                throw new NotImplementedException();
            }
        }

        protected override string GetChildName(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.GetChildName(string)");
            try
            {
                return Path.GetChildName(provider: this, rawPath: rawPath);
            }
            catch
            {
                return (rawPath ?? string.Empty)
                    .Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault() ?? string.Empty;
            }
        }

        protected override void GetItem(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.GetItem(...)");
            Path path = this.ParsePath(rawPath);
            foreach (PSObject psObject in path.GetDriveItem())
            {
                string relativePath;
                Segment parentSegment = psObject.GetPSVsoParentSegment();
                if (parentSegment == null)
                {
                    relativePath = string.Empty;
                }
                else
                {
                    relativePath =
                        string.Format(
                            CultureInfo.InvariantCulture,
                            @"{0}\{1}",
                            psObject.GetPSVsoParentSegment().RelativePath,
                            psObject.GetPSVsoName())
                        .TrimStart('\\');
                }

                this.WriteItemObject(
                    item: psObject,
                    path: relativePath,
                    isContainer: psObject.GetPSVsoIsContainer());
            }
        }

        protected override string GetParentPath(string rawPath, string root)
        {
            this.WriteVerbose("VsoProvider.Provider.GetParentPath(...)");
            Path path = this.ParsePath(rawPath);
            Segment lastSegment = path.Segments.Last();
            Segment lastParentSegment = lastSegment.GetParent();
            if (lastParentSegment == null)
            {
                return string.Empty;
            }

            return rawPath.Substring(
                    startIndex: 0,
                    length: rawPath.Length - lastSegment.Name.Length)
                .TrimEnd('\\');
        }

        protected override bool IsItemContainer(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.IsItemContainer(...)");
            try
            {
                Path path = this.ParsePath(rawPath);
                Segment lastSegment = path.Segments.Last();
                if (lastSegment.ItemTypeInfo is ContainerTypeInfo)
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        protected override bool IsValidPath(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.IsValidPath(...)");
            try
            {
                this.ParsePath(rawPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool ItemExists(string rawPath)
        {
            this.WriteVerbose("VsoProvider.Provider.ItemExists(...)");
            try
            {
                Path path = this.ParsePath(rawPath);
                if (path.GetDriveItem().Any())
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo driveInfo)
        {
            this.WriteVerbose("VsoProvider.Provider.NewDrive(...)");
            return new DriveInfo(driveInfo, this.DynamicParameters as DriveParameters);
        }

        protected override object NewDriveDynamicParameters()
        {
            this.WriteVerbose("VsoProvider.Provider.NewDriveDynamicParameters()");
            return new DriveParameters();
        }

        protected override void NewItem(string rawPath, string itemTypeName, object newItemValue)
        {
            this.WriteVerbose("VsoProvider.Provider.NewItem(...)");
            Path path = this.ParsePath(rawPath);
            path.NewItem(this.DynamicParameters);
        }

        protected override object NewItemDynamicParameters(string rawPath, string itemTypeName, object newItemValue)
        {
            this.WriteVerbose("VsoProvider.Provider.NewItemDynamicParameters(...)");
            Path path = this.ParsePath(rawPath);
            return path.NewItemDynamicParameters();
        }

        private Path ParsePath(string rawPath)
        {
            return new Path(provider: this, rawPath: rawPath);
        }
    }
}