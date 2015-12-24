namespace VsoProvider.DriveItems
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using System.Text;

    public abstract class TypeInfo
    {
        public abstract string Name { get; }

        public static IEnumerable<PSObject> ToJson(PSObject psObject)
        {
            Segment parentSegment = psObject.GetPSVsoParentSegment();
            const string Script = @"
[cmdletbinding()]
param(
    [psobject]$PSObject = $(throw 'Missing PSObject.')
)

$PSObject |
    Select-Object -Property * -ExcludeProperty PS* |
    ConvertTo-Json -Depth 1000
";
            return parentSegment.GetProvider().InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] { psObject });
        }

        public virtual PSObject ConvertToDriveItem(Segment parentSegment, object obj)
        {
            PSObject psObject =
                obj is PSObject
                ? obj as PSObject
                : new PSObject(obj ?? new object());
            psObject.AddPSVsoIsContainer(this is ContainerTypeInfo);
            psObject.AddPSVsoParentSegment(parentSegment);
            if (parentSegment == null)
            {
                Provider provider = obj as Provider;
                if (provider == null)
                {
                    throw new NotSupportedException("Code should not reach here.");
                }

                psObject.AddPSVsoProvider(provider);
            }
            else
            {
                psObject.AddPSVsoProvider(parentSegment.GetProvider());
            }

            psObject.AddPSVsoTypeInfo(this);
            psObject.Methods.Add(new PSCodeMethod("ToJson", typeof(TypeInfo).GetMethod("ToJson", BindingFlags.Public | BindingFlags.Static)));
            return psObject;
        }

        protected IEnumerable<PSObject> InvokeDeleteWebRequest(Provider provider, string relativeUrl)
        {
            // Determine the URL.
            string serverUrl = provider.PSVsoDriveInfo.Root.TrimEnd('/');
            string url = string.Format("{0}/{1}", serverUrl, relativeUrl);

            // Initialize the headers.
            Dictionary<string, string> headers = InitializeHeaders(provider, isPostOrPut: false);

            // Format the script.
            const string Script = @"
[cmdletbinding()]
param(
    [string]$Url = $(throw 'Missing Url.'),
    [hashtable]$Headers = $(throw 'Missing Headers.')
)

[bool]$useDefaultCredentials = ""$($Headers.Authorization)"" -eq ''
$response = Invoke-WebRequest -Uri $Url -UseBasicParsing -Method Delete -UseDefaultCredentials:$useDefaultCredentials -Headers $Headers -Body $BodyJson
if ($response.StatusCode -ne 204) {
    throw ""Unexpected response code: $($response.StatusCode) and status description: $($response.StatusDescription)""
}

ConvertFrom-Json $response.Content
";

            // Invoke the script.
            return provider.InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] { url, headers });
        }

        protected IEnumerable<PSObject> InvokePostWebRequest(Provider provider, string relativeUrl, string bodyJson)
        {
            // Determine the URL.
            string serverUrl = provider.PSVsoDriveInfo.Root.TrimEnd('/');
            string url = string.Format("{0}/{1}", serverUrl, relativeUrl);

            // Initialize the headers.
            Dictionary<string, string> headers = InitializeHeaders(provider, isPostOrPut: true);

            // Format the script.
            const string Script = @"
[cmdletbinding()]
param(
    [string]$Url = $(throw 'Missing Url.'),
    [hashtable]$Headers = $(throw 'Missing Headers.'),
    [string]$BodyJson = $(throw 'Missing BodyJson.')
)

[bool]$useDefaultCredentials = ""$($Headers.Authorization)"" -eq ''
$response = Invoke-WebRequest -Uri $Url -UseBasicParsing -Method Post -UseDefaultCredentials:$useDefaultCredentials -Headers $Headers -Body $BodyJson
if (($response.StatusCode -ne 200) -and ($response.StatusCode -ne 201) -and ($response.StatusCode -ne 202)) {
    throw ""Unexpected response code: $($response.StatusCode) and status description: $($response.StatusDescription)""
}

ConvertFrom-Json $response.Content
";

            // Invoke the script.
            return provider.InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] { url, headers, bodyJson });
        }

        protected IEnumerable<PSObject> InvokePutWebRequest(Provider provider, string relativeUrl, PSObject psObject)
        {
            // Determine the URL.
            string serverUrl = provider.PSVsoDriveInfo.Root.TrimEnd('/');
            string url = string.Format("{0}/{1}", serverUrl, relativeUrl);

            // Initialize the headers.
            Dictionary<string, string> headers = InitializeHeaders(provider, isPostOrPut: true);

            // Format the script.
            const string Script = @"
[cmdletbinding()]
param(
    [string]$Url = $(throw 'Missing Url.'),
    [hashtable]$Headers = $(throw 'Missing Headers.'),
    $BodyObject = $(throw 'Missing BodyObject.')
)

[string]$bodyJson = ConvertTo-Json -Depth 1000 $BodyObject
[bool]$useDefaultCredentials = ""$($Headers.Authorization)"" -eq ''
$response = Invoke-WebRequest -Uri $Url -UseBasicParsing -Method Put -UseDefaultCredentials:$useDefaultCredentials -Headers $Headers -Body $bodyJson
if ($response.StatusCode -ne 200) {
    throw ""Unexpected response code: $($response.StatusCode) and status description: $($response.StatusDescription)""
}

ConvertFrom-Json $response.Content
";

            // Invoke the script.
            return provider.InvokeCommand.InvokeScript(
                script: Script,
                useNewScope: true,
                writeToPipeline: PipelineResultTypes.None,
                input: null,
                args: new object[] { url, headers, psObject });
        }

        protected string JsonStringFormat(string format, params object[] unencodedArgs)
        {
            List<string> encodedArgs = new List<string>();
            foreach (object unencodedArg in unencodedArgs ?? new object[0])
            {
                string encodedArg;
                if (unencodedArg is string)
                {
                    encodedArg =
                        (unencodedArg as string ?? string.Empty)
                        .Replace(@"\", @"\\")
                        .Replace(@"""", @"\""")
                        .Replace("\b", @"\b")
                        .Replace("\f", @"\f")
                        .Replace("\n", @"\n")
                        .Replace("\r", @"\r")
                        .Replace("\t", @"\t");
                }
                else if (unencodedArg is int)
                {
                    encodedArg = ((int)unencodedArg).ToString();
                }
                else if (object.ReferenceEquals(unencodedArg, null))
                {
                    encodedArg = string.Empty;
                }
                else
                {
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "Unexpected unencoded object type: {0}",
                        unencodedArg.GetType().FullName);
                    throw new NotSupportedException(message);
                }

                encodedArgs.Add(encodedArg);
            }

            return string.Format(CultureInfo.InvariantCulture, format, encodedArgs.ToArray());
        }

        protected string UrlStringFormat(string format, params object[] unencodedArgs)
        {
            // Determine the URL.
            string[] encodedArgs =
                (unencodedArgs ?? new object[0])
                .Select(x => string.Format(CultureInfo.InvariantCulture, "{0}", x))
                .Select(x => Uri.EscapeUriString(x))
                .ToArray();
            return string.Format(CultureInfo.InvariantCulture, format, encodedArgs);
        }

        private static Dictionary<string, string> InitializeHeaders(Provider provider, bool isPostOrPut)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (provider.PSVsoDriveInfo.UsePersonalAccessToken)
            {
                headers["Authorization"] = string.Format(
                    "Basic {0}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(":{0}", SecureStringHelper.ConvertToString(provider.PSVsoDriveInfo.PersonalAccessToken)))));
            }

            if (isPostOrPut)
            {
                headers["Content-Type"] = "application/json";
            }

            return headers;
        }
    }
}
