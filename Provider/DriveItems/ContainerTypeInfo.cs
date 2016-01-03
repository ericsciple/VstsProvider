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

        public virtual IEnumerable<PSObject> GetItems(Segment segment)
        {
            return this.ChildTypeInfo
                .Values
                .Where(x => x is HttpClientContainerTypeInfo)
                .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(x => x.ConvertToDriveItem(parentSegment: segment, obj: null));
        }

        public virtual IEnumerable<PSObject> GetItemByWildcard(Segment segment, Segment childSegment)
        {
            WildcardPattern pattern = new WildcardPattern(
                pattern: childSegment.Name,
                options: WildcardOptions.IgnoreCase);
            return this.GetItems(segment)
                .Where(x => pattern.IsMatch(x.GetPSVstsChildName()));
        }

        public virtual IEnumerable<PSObject> GetLiteralItem(Segment segment, Segment childSegment)
        {
            return this.GetItems(segment)
                .Where(x => string.Equals(x.GetPSVstsChildName(), childSegment.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        protected void AddChildTypeInfo(TypeInfo childTypeInfo)
        {
            this.ChildTypeInfo.Add(childTypeInfo.Name, childTypeInfo);
        }
    }
}
