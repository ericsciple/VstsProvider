namespace VstsProvider.DriveItems
{
    using System.Collections.Generic;
    using System.Linq;

    public class Segment
    {
        private readonly Segment parent;
        private readonly Path path;
        private readonly Provider provider;

        public Segment(Provider provider, Path path, TypeInfo typeInfo, string name, Queue<string> remainingNames, Segment parent)
        {
            this.provider = provider;
            //this.provider.WriteDebug("DriveItems.Segment.ctor(...)");
            this.path = path;
            this.ItemTypeInfo = typeInfo;
            this.Name = name;
            this.parent = parent;
            //this.provider.WriteDebug(string.Format(" this.Name={0}", this.Name));
            //this.provider.WriteDebug(string.Format(" this.Parent.Name={0}", this.GetParent() == null ? string.Empty : this.GetParent().Name));
            //this.provider.WriteDebug(string.Format(" remainingNames={0}", remainingNames));
            if (remainingNames.Count == 0)
            {
                return;
            }

            // Validate the item type info is known for this segment.
            if (this.ItemTypeInfo == null)
            {
                this.path.ThrowInvalid("Unknown path segment type cannot contain a child segment.");
            }

            // Validate this segment is a container.
            if (this.ItemTypeInfo is LeafTypeInfo)
            {
                this.path.ThrowInvalid("Leaf path segments cannot contain a child segment.");
            }

            //// Validate this segment is not a wildcard.
            //if (this.HasWildcard)
            //{
            //    this.path.ThrowInvalid("Wildcard must appear in the last segment only.");
            //}

            // Determine the child segment's type info.
            string childName = remainingNames.Dequeue();
            Dictionary<string, TypeInfo> childTypeInfos = (this.ItemTypeInfo as ContainerTypeInfo).ChildTypeInfo;
            TypeInfo childTypeInfo;
            //this.provider.WriteDebug(string.Format("Determining type info for child segment: {0}", childName));
            if (childTypeInfos.ContainsKey(childName)
                && childTypeInfos[childName] is HttpClientContainerTypeInfo)
            {
                childTypeInfo = childTypeInfos[childName];
            }
            else if (childTypeInfos.Count == 1
                && !(childTypeInfos.Values.Single() is HttpClientContainerTypeInfo))
            {
                childTypeInfo = childTypeInfos.Values.Single();
            }
            else
            {
                childTypeInfo = null;
            }

            // Create the child segment.
            this.Child = new Segment(
                provider: this.provider,
                path: this.path,
                typeInfo: childTypeInfo,
                name: childName,
                remainingNames: remainingNames,
                parent: this);
        }

        public Segment Child { get; private set; }

        // public bool HasWildcard
        // {
        //     get
        //     {
        //         return CheckHasWildcard(this.Name);
        //     }
        // }

        public TypeInfo ItemTypeInfo { get; private set; }

        public string Name { get; private set; }

//         // TODO: ADD BETTER PROPERTIES (DrivePath, ParentDrivePath) THAT ARE ROOTED WITH THE DRIVE NAME.
//         // TODO: NOT SURE THIS SHOULD BE ROOTED WITH A SLASH.
//         public string RelativePath
//         {
//             get
//             {
//                 Stack<string> segmentNames = new Stack<string>();
//                 Segment segment = this;
//                 while (!(segment is RootSegment))
//                 {
//                     segmentNames.Push(segment.Name);
//                     segment = segment.GetParent();
//                 }
// 
//                 return segmentNames.Count == 0
//                     ? string.Empty
//                     : string.Concat(@"\", string.Join(separator: @"\", values: segmentNames));
//             }
//         }
        public string UnescapedName
        {
            get
            {
                return SegmentHelper.Unescape(this.Name);
            }
        }

        public Segment GetParent()
        {
            return this.parent;
        }

        // public Path GetPath()
        // {
        //     return this.path;
        // }

        public Provider GetProvider()
        {
            return this.provider;
        }

//         private static bool CheckHasWildcard(string name)
//         {
//             foreach (char c in name)
//             {
//                 if (c == '*'
//                     || c == '?'
//                     || c == '['
//                     || c == ']')
//                 {
//                     return true;
//                 }
//             }
// 
//             return false;
//         }
    }
}
