namespace VstsProvider
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Provider;
    using VstsProvider.DriveItems;

    [CmdletProvider("VSTS", ProviderCapabilities.ExpandWildcards)]
    public sealed class Provider : NavigationCmdletProvider
    {
        public DriveInfo PSVstsDriveInfo
        {
            get
            {
                return this.PSDriveInfo as DriveInfo;
            }
        }

        public void WriteDebug(string format, params object[] args)
        {
            string message =
                args.Length == 0
                ? format
                : string.Format(CultureInfo.InvariantCulture, format, args);
            base.WriteDebug(message);
        }

        public void WriteVerbose(string format, params object[] args)
        {
            string message =
                args.Length == 0
                ? format
                : string.Format(CultureInfo.InvariantCulture, format, args);
            base.WriteVerbose(message);
        }

        protected override string[] ExpandPath(string wildcardPath)
        {
            this.WriteDebug("VstsProvider.Provider.ExpandPath(wildcardPath: '{0}')", wildcardPath);
            Path path = this.ParsePath(wildcardPath);
            string parentPath = path.PSParentPath;
            return path.GetItemByWildcard()
                .Select(x => Combine(parentPath, x.GetPSVstsChildName()))
                .ToArray();
        }

        protected override void GetChildItems(string rawPath, bool recurse)
        {
            this.WriteDebug("VstsProvider.Provider.GetChildItems(path: '{0}', recurse: {1})", rawPath, recurse);
            if (recurse)
            {
                throw new NotSupportedException("Recursion not supported.");
            }

            Path path = this.ParsePath(string.Concat(rawPath, @"\*"));
            string parentPath = path.PSParentPath;
            foreach (PSObject psObject in path.GetItemByWildcard())
            {
                this.WriteItemObject(
                    item: psObject,
                    path: Combine(parentPath, psObject.GetPSVstsChildName()),
                    isContainer: psObject.GetPSVstsIsContainer());
            }
        }

        protected override string GetChildName(string rawPath)
        {
            this.WriteDebug("VstsProvider.Provider.GetChildName(path: '{0}')", rawPath);
            // try
            // {
            string childName = this.ParsePath(rawPath).PSChildName;
            //this.WriteDebug(" Result: '{0}'", childName);
            return childName;
            // }
            // catch
            // {
            //     return (rawPath ?? string.Empty)
            //         .Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries)
            //         .LastOrDefault() ?? string.Empty;
            // }
        }

        protected override void GetItem(string literalPath)
        {
            this.WriteDebug("VstsProvider.Provider.GetItem(literalPath: '{0}')", literalPath);
            Path path = this.ParsePath(literalPath);
            foreach (PSObject psObject in path.GetLiteralItem())
            {
                this.WriteItemObject(
                    item: psObject,
                    path: path.PSPath,
                    isContainer: psObject.GetPSVstsIsContainer());
            }
        }

        protected override string GetParentPath(string rawPath, string root)
        {
             this.WriteDebug("VstsProvider.Provider.GetParentPath(path: '{0}', root: '{1}')", rawPath, root);
            string parentPath = this.ParsePath(rawPath).PSParentPath;
            //this.WriteDebug(" Result: '{0}'", parentPath);
            return parentPath;
        }

        protected override bool IsItemContainer(string literalPath)
        {
            this.WriteDebug("VstsProvider.Provider.IsItemContainer(literalPath: '{0}')", literalPath);
            try
            {
                Path path = this.ParsePath(literalPath);
                return path.Segments.Last().ItemTypeInfo is ContainerTypeInfo;
            }
            catch
            {
            }

            return false;
        }

        protected override bool IsValidPath(string rawPath)
        {
            this.WriteDebug("VstsProvider.Provider.IsValidPath(path: '{0}')", rawPath);
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

        protected override bool ItemExists(string literalPath)
        {
            this.WriteDebug("VstsProvider.Provider.ItemExists(literalPath: '{0}')", literalPath);
            try
            {
                Path path = this.ParsePath(literalPath);
                return path.GetLiteralItem().Any();
            }
            catch
            {
            }

            return false;
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo driveInfo)
        {
            this.WriteDebug("VstsProvider.Provider.NewDrive(...)");
            return new DriveInfo(driveInfo, this.DynamicParameters as DriveParameters);
        }

        protected override object NewDriveDynamicParameters()
        {
            this.WriteDebug("VstsProvider.Provider.NewDriveDynamicParameters()");
            return new DriveParameters();
        }

        private static string Combine(string p1, string p2)
        {
            string separator =
                p1.EndsWith(@"\")
                ? string.Empty
                : @"\";
            return string.Concat(p1, separator, p2);
        }

        private Path ParsePath(string rawPath)
        {
            return new Path(provider: this, rawPath: rawPath);
        }
    }
}