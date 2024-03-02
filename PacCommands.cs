using System;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikeFactorial.XTB.PPCLIx
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

        public static string[] RetrieveUsageDetails(string results, string commandText)
        {
            if ((results.Contains("[") || results.Contains("--")) && !results.Contains("Error:"))
            {
                string usageDetails = results
                    .Split('\n')
                    .FirstOrDefault(s => s.Contains($"{commandText} [") || s.Contains($"{commandText} --"));

                if (!string.IsNullOrEmpty(usageDetails))
                {
                    string delim = "[";
                    if (usageDetails.Contains($"{commandText} [--") || usageDetails.Contains($"{commandText} --"))
                    {
                        delim = "--";
                    }
                    //Handle nouns and verbs
                    return usageDetails
                        .Substring(usageDetails.IndexOf(delim), (usageDetails.Length) - usageDetails.IndexOf(delim)).Replace("[", "").Replace("]", "").Replace("\r", "")
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
                if (lines[i].Trim().StartsWith($"{nodeText} "))
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
