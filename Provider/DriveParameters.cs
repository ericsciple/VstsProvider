namespace VsoProvider
{
    using System.Management.Automation;

    public class DriveParameters
    {
        public DriveParameters()
        {
        }

        [Parameter()]
        public string PersonalAccessToken { get; set; }

        [Parameter()]
        public SwitchParameter UseDefaultCredentials { get; set; }

        [Parameter()]
        public SwitchParameter UsePersonalAccessToken { get; set; }
    }
}
