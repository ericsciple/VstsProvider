namespace VsoProvider
{
    using System.Management.Automation;
    using System.Security;

    public sealed class DriveInfo : PSDriveInfo
    {
        private readonly DriveParameters driveParameters;

        public DriveInfo(PSDriveInfo driveInfo, DriveParameters driveParameters) : base(driveInfo)
        {
            // Store the drive parameters.
            this.driveParameters = driveParameters;

            // Store the personal access token as secure string and set the unsecure string to null.
            if (!string.IsNullOrEmpty(this.driveParameters.PersonalAccessToken))
            {
                this.PersonalAccessToken = SecureStringHelper.ConvertToSecureString(this.driveParameters.PersonalAccessToken);
                this.driveParameters.PersonalAccessToken = null;
            }
        }

        public SecureString PersonalAccessToken { get; private set; }

        public bool UseDefaultCredentials
        {
            get
            {
                return !this.UsePersonalAccessToken;
            }
        }

        public bool UsePersonalAccessToken
        {
            get
            {
                return this.driveParameters.UsePersonalAccessToken.IsPresent
                    || this.PersonalAccessToken != null;
            }
        }

        public string GetPersonalAccessToken()
        {
            if (!this.UsePersonalAccessToken)
            {
                return null;
            }

            return SecureStringHelper.ConvertToString(this.PersonalAccessToken);
        }
    }
}
