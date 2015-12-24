namespace VstsProvider
{
    using System.Management.Automation;
    using DriveItems;

    public static class ExtensionMethods
    {
        public static void AddPSVstsIsContainer(this PSObject psObject, bool value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVstsIsContainer", value));
        }

        public static void AddPSVstsName(this PSObject psObject, string value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVstsName", value));
        }

        public static void AddPSVstsParentSegment(this PSObject psObject, Segment value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVstsParentSegment", value));
        }

        public static void AddPSVstsProvider(this PSObject psObject, Provider value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVstsProvider", value));
        }

        public static void AddPSVstsTypeInfo(this PSObject psObject, TypeInfo value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVstsTypeInfo", value));
        }

        public static bool GetPSVstsIsContainer(this PSObject psObject)
        {
            return (bool)psObject.Properties["PSVstsIsContainer"].Value;
        }

        public static string GetPSVstsName(this PSObject psObject)
        {
            return psObject.Properties["PSVstsName"].Value as string;
        }

        public static Segment GetPSVstsParentSegment(this PSObject psObject)
        {
            return psObject.Properties["PSVstsParentSegment"].Value as Segment;
        }

        public static Provider GetPSVstsProvider(this PSObject psObject)
        {
            return psObject.Properties["PSVstsProvider"].Value as Provider;
        }

        public static TypeInfo GetPSVstsTypeInfo(this PSObject psObject)
        {
            return psObject.Properties["PSVstsTypeInfo"].Value as TypeInfo;
        }
    }
}