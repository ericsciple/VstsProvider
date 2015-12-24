namespace VsoProvider
{
    using System.Management.Automation;
    using DriveItems;

    public static class ExtensionMethods
    {
        public static void AddPSVsoIsContainer(this PSObject psObject, bool value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVsoIsContainer", value));
        }

        public static void AddPSVsoName(this PSObject psObject, string value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVsoName", value));
        }

        public static void AddPSVsoParentSegment(this PSObject psObject, Segment value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVsoParentSegment", value));
        }

        public static void AddPSVsoProvider(this PSObject psObject, Provider value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVsoProvider", value));
        }

        public static void AddPSVsoTypeInfo(this PSObject psObject, TypeInfo value)
        {
            psObject.Properties.Add(new PSNoteProperty("PSVsoTypeInfo", value));
        }

        public static bool GetPSVsoIsContainer(this PSObject psObject)
        {
            return (bool)psObject.Properties["PSVsoIsContainer"].Value;
        }

        public static string GetPSVsoName(this PSObject psObject)
        {
            return psObject.Properties["PSVsoName"].Value as string;
        }

        public static Segment GetPSVsoParentSegment(this PSObject psObject)
        {
            return psObject.Properties["PSVsoParentSegment"].Value as Segment;
        }

        public static Provider GetPSVsoProvider(this PSObject psObject)
        {
            return psObject.Properties["PSVsoProvider"].Value as Provider;
        }

        public static TypeInfo GetPSVsoTypeInfo(this PSObject psObject)
        {
            return psObject.Properties["PSVsoTypeInfo"].Value as TypeInfo;
        }
    }
}