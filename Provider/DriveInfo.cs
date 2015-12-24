namespace VstsProvider
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security;
    using Microsoft.VisualStudio.Services.Common;

    public sealed class DriveInfo : PSDriveInfo
    {
        private readonly DriveParameters driveParameters;
        private readonly Dictionary<Type, object> httpClients = new Dictionary<Type, object>();
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

        public T GetHttpClient<T>() where T : class
        {
            object httpClient;
            if (!this.httpClients.TryGetValue(typeof(T), out httpClient))
            {
                ConstructorInfo constructorInfo = typeof(T).GetConstructor(new[] { typeof(Uri), typeof(VssCredentials) });
                httpClient = constructorInfo.Invoke(
                    new object[] { new Uri(this.Root), this.VssCredentials });
                this.httpClients[typeof(T)] = httpClient;
            }

            return httpClient as T;
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
