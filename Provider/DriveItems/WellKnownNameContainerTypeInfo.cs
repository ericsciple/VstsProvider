namespace VsoProvider.DriveItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;
    using System.Management.Automation;
    using System.Text;
    using System.Management.Automation.Runspaces;

    public abstract class WellKnownNameContainerTypeInfo : ContainerTypeInfo
    {
        public override PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject = base.ConvertToDriveItem(parentSegment, obj);
            psObject.AddPSVsoName(this.Name);
            return psObject;
        }

        // TODO: PASS FLAG INDICATING: SINGLE_ITEM, FIRST_PAGE, ALL_PAGES. DEFINE ABSTRACT PROPERTIES INDICATING WHETHER PAGING IS BY CONTINUATION TOKEN OR $TOP/$SKIP AND THE PAGE SIZE.
        protected IEnumerable<PSObject> InvokeGetWebRequest(Segment parentSegment, string relativeUrlFormat, params object[] unencodedArgs)
        {
            // Determine the URL.
            string relativeUrl = this.UrlStringFormat(relativeUrlFormat, unencodedArgs);
            string serverUrl = parentSegment.GetProvider().PSVsoDriveInfo.Root.TrimEnd('/');
            string originalUrl = string.Format("{0}/{1}", serverUrl, relativeUrl);

            string continuationToken = null;
            const string ContinuationTokenPrefix = "x-ms-continuationtoken=";
            do
            {
                // Format the URL with the continuation token if required.
                string url;
                if (string.IsNullOrEmpty(continuationToken))
                {
                    url = originalUrl;
                }
                else
                {
                    url = string.Format("{0}&continuationToken={1}", originalUrl, continuationToken);
                }

                // Format the script.
                const string ScriptTextFormat = @"
$response = Invoke-WebRequest {0} -UseBasicParsing -Uri '{1}'
if ($response.StatusCode -ne 200) {{
    throw ""Unexpected response code: $($response.StatusCode) and status description: $($response.StatusDescription)""
}}

if ($response.Headers['x-ms-continuationtoken']) {{
    ""{2}$($response.Headers['x-ms-continuationtoken'])""
}}

ConvertFrom-Json $response.Content |
    Foreach-Object {{
        if ($_.count -and $_.value) {{
            $_.value
        }} elseif ($_.count -eq 0) {{
        }} else {{
            $_
        }}
    }}
";
                string scriptText = string.Format(
                    CultureInfo.InvariantCulture,
                    ScriptTextFormat,
                    GetCredentials(parentSegment.GetProvider()),
                    url,
                    ContinuationTokenPrefix);

                // Reset the continuation token.
                continuationToken = null;

                // Invoke the script.
                foreach (PSObject psObject in parentSegment.GetProvider().InvokeCommand.InvokeScript(scriptText))
                {
                    // Check for continuation token.
                    if ((psObject.BaseObject as string ?? string.Empty).StartsWith("x-ms-continuationtoken=", StringComparison.OrdinalIgnoreCase))
                    {
                        continuationToken = (psObject.BaseObject as string).Substring(startIndex: ContinuationTokenPrefix.Length);
                        continue;
                    }

                    yield return this.ChildTypeInfo.Values.Single().ConvertToDriveItem(parentSegment, psObject);
                }
            } while (!string.IsNullOrEmpty(continuationToken));
        }

        private static string GetCredentials(Provider provider)
        {
            if (provider.PSVsoDriveInfo.UsePersonalAccessToken)
            {
                return string.Format(
                    "-Headers @{{ 'Authorization' = 'Basic {0}' }}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(":{0}", SecureStringHelper.ConvertToString(provider.PSVsoDriveInfo.PersonalAccessToken)))));
            }
            else
            {
                return "-UseDefaultCredentials";
            }
        }
    }
}
