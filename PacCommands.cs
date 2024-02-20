using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikeFactorial.XTB.PACUI
{
    public class PacCommands
    {
        public static int RetrieveAuthIndex(string auth)
        {
            int authIndex;
            if (Int32.TryParse(auth.Substring(0, auth.IndexOf("]")).Replace("[", "").Replace("]", ""), out authIndex))
            {
                return authIndex;
            }
            return -1;
        }

        public static string[] RetrieveUsageDetails(string results)
        {
            if (results.Contains("[") && !results.Contains("Error:"))
            {
                string usageDetails = results
                    .Split('\n')
                    .FirstOrDefault(s => s.StartsWith("Usage:"));

                if (!string.IsNullOrEmpty(usageDetails))
                {
                    return usageDetails
                        .Substring(usageDetails.IndexOf("["), (usageDetails.LastIndexOf("]") + 1) - usageDetails.IndexOf("[")).Replace("[", "").Replace("]", "")
                        .Split(' ');
                }
            }
            return new string[] { };
        }

        public static object GetDefaultArgumentValue(string helpText)
        {
            return string.Empty;
        }

        public static string RetrieveNodeHelpText(string results, string nodeText)
        {
            var lines = results.Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith(nodeText))
                {
                    if (lines.Length > i && lines[i + 1].Trim().StartsWith("Values"))
                    {
                        return lines[i].Trim() + "\n" + lines[i + 1].Trim();
                    }
                    return lines[i];
                }
            }
            return string.Empty;
        }



    }
}
