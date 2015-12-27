namespace VstsProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using Microsoft.VisualStudio.Services.Common;

    public sealed class DriveInfo : PSDriveInfo
    {
        private readonly DriveParameters driveParameters;
        private VssCredentials vssCredentials;

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

        private VssCredentials VssCredentials
        {
            get
            {
                if (this.UsePersonalAccessToken)
                {
                    throw new NotImplementedException();
                }

                return new VssCredentials();
            }
        }

        public T GetHttpClient<T>(params string[] segments) where T : class
        {
            StringBuilder url = new StringBuilder();
            url.Append((this.Root ?? string.Empty).TrimEnd('/'));
            foreach (string segment in segments)
            {
                url.Append('/');
                url.Append(Uri.EscapeDataString(segment));
            }

            ConstructorInfo constructorInfo = typeof(T).GetConstructor(
                new[] { typeof(Uri), typeof(VssCredentials) });
            return constructorInfo.Invoke(
                new object[] { new Uri(url.ToString()), this.VssCredentials }) as T;
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
