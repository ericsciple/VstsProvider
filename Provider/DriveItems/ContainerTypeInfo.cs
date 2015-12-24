namespace VstsProvider.DriveItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    public abstract class ContainerTypeInfo : TypeInfo
    {
        readonly Dictionary<string, TypeInfo> childTypeInfo = new Dictionary<string, TypeInfo>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, TypeInfo> ChildTypeInfo
        {
            get
            {
                return this.childTypeInfo;
            }
        }

        public virtual IEnumerable<PSObject> GetChildDriveItems(Segment segment)
        {
            return this.ChildTypeInfo
                .Values
                .Where(x => x is WellKnownNameContainerTypeInfo)
                .OrderBy(x => x.Name)
                .Select(x => x.ConvertToDriveItem(parentSegment: segment, obj: x.Name));
        }

        public virtual IEnumerable<PSObject> GetChildDriveItems(Segment segment, Segment childSegment)
        {
            // Handle wildcard child segment.
            if (childSegment.HasWildcard)
            {
                WildcardPattern pattern;
                segment.GetProvider().WriteVerbose("childSegment.HasWildcard");
                pattern = new WildcardPattern(
                    pattern: childSegment.Name,
                    options: WildcardOptions.CultureInvariant | WildcardOptions.IgnoreCase);
                return this.GetChildDriveItems(segment)
                    .Where(x => pattern.IsMatch(x.GetPSVstsName()));
            }

            // Handle non-wildcard child segment.
            return this.GetChildDriveItems(segment)
                .Where(x => string.Equals(childSegment.Name, x.GetPSVstsName(), StringComparison.OrdinalIgnoreCase));
        }

        public virtual IEnumerable<PSObject> NewChildDriveItem(Segment segment, Segment childSegment, object dynamicParameters)
        {
            throw new NotSupportedException();
        }

        public virtual object NewChildDriveItemDynamicParameters()
        {
            return new object();
        }

        protected void AddChildTypeInfo(TypeInfo childTypeInfo)
        {
            this.ChildTypeInfo.Add(childTypeInfo.Name, childTypeInfo);
        }
    }
}
