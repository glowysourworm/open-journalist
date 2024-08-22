using System;
using System.Diagnostics;

namespace OpenJournalist.Utility
{
    public static class HyperlinkUtility
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
