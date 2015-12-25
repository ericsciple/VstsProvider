namespace VstsProvider.DriveItems
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
            psObject.AddPSVstsName(this.Name);
            return psObject;
        }

        protected PSObject ConvertToChildDriveItem(Segment parentSegment, object obj)
        {
            return this.ChildTypeInfo.Values.Single().ConvertToDriveItem(parentSegment, obj);
        }

        // TODO: PASS FLAG INDICATING: SINGLE_ITEM, FIRST_PAGE, ALL_PAGES. DEFINE ABSTRACT PROPERTIES INDICATING WHETHER PAGING IS BY CONTINUATION TOKEN OR $TOP/$SKIP AND THE PAGE SIZE.
        protected IEnumerable<PSObject> InvokeGetWebRequest(Segment parentSegment, string relativeUrlFormat, params object[] unencodedArgs)
        {
            // Determine the URL.
            string relativeUrl = this.UrlStringFormat(relativeUrlFormat, unencodedArgs);
            string serverUrl = parentSegment.GetProvider().PSVstsDriveInfo.Root.TrimEnd('/');
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

        protected T Wrap<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerExceptions[0];
                }

                throw;
            }
        }

        protected IEnumerable<T> Wrap<T>(Func<int?, int?, IEnumerable<T>> func)
        {
            int count = 0;
            int? top = null;
            int? skip = null;
            while (true)
            {
                // Execute the func and unwrap aggregate exception if only one exception.
                T[] results;
                try
                {
                    results = func(top, skip).ToArray();
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Count == 1)
                    {
                        throw ex.InnerExceptions[0];
                    }

                    throw;
                }

                // Yield and count the results.
                int intermediateCount = 0;
                foreach (T result in results)
                {
                    intermediateCount++;
                    count++;
                    yield return result;
                }

                // Set top if not set.
                if (top == null)
                {
                    top = intermediateCount;
                }

                // Short-circuit if done.
                if (intermediateCount == 0 || intermediateCount < top.Value)
                {
                    break;
                }

                // Set skip.
                skip = count;
            }
        }

        private static string GetCredentials(Provider provider)
        {
            if (provider.PSVstsDriveInfo.UsePersonalAccessToken)
            {
                return string.Format(
                    "-Headers @{{ 'Authorization' = 'Basic {0}' }}",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format(":{0}", SecureStringHelper.ConvertToString(provider.PSVstsDriveInfo.PersonalAccessToken)))));
            }
            else
            {
                return "-UseDefaultCredentials";
            }
        }
    }
}
