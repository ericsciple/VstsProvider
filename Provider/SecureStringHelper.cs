namespace VsoProvider
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringHelper
    {
        [SecurityCritical]
        public static SecureString ConvertToSecureString(string str)
        {
            SecureString secureString = new SecureString();
            foreach (char c in str)
            {
                secureString.AppendChar(c);
            }

            return secureString;
        }

        [SecurityCritical]
        public static string ConvertToString(SecureString secureString)
        {
            using (SecureString secureString2 = secureString.Copy())
            {
                unsafe
                {
                    IntPtr bstr = Marshal.SecureStringToBSTR(secureString);
                    try
                    {
                        return new string((char*)bstr);
                    }
                    finally
                    {
                        Marshal.ZeroFreeBSTR(bstr);
                    }
                }
            }
        }
    }
}