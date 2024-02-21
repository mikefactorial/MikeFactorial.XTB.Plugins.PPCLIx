using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikeFactorial.XTB.PPCLIx
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CatalogEntry
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string authors { get; set; }
        public List<DependencyGroup> dependencyGroups { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string language { get; set; }
        public string licenseExpression { get; set; }
        public string licenseUrl { get; set; }
        public bool listed { get; set; }
        public string minClientVersion { get; set; }
        public string packageContent { get; set; }
        public string projectUrl { get; set; }
        public DateTime published { get; set; }
        public bool requireLicenseAcceptance { get; set; }
        public string summary { get; set; }
        public List<string> tags { get; set; }
        public string title { get; set; }
        public string version { get; set; }
        public string readmeUrl { get; set; }
    }

    public class CommitId
    {
        [JsonProperty("@id")]
        public string id { get; set; }
    }

    public class CommitTimeStamp
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
    }
    public class Count
    {
        [JsonProperty("@id")]
        public string id { get; set; }
    }

    public class Dependencies
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@container")]
        public string container { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string range { get; set; }
        public string registration { get; set; }
    }

    public class DependencyGroup
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string targetFramework { get; set; }

        [JsonProperty("@container")]
        public string container { get; set; }
    }

    public class Item
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
        public string commitId { get; set; }
        public DateTime commitTimeStamp { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }
        public string parent { get; set; }
        public string lower { get; set; }
        public string upper { get; set; }
        public CatalogEntry catalogEntry { get; set; }
        public string packageContent { get; set; }
        public string registration { get; set; }

        [JsonProperty("@container")]
        public string container { get; set; }
    }

    public class PackageContent
    {
        [JsonProperty("@type")]
        public string type { get; set; }
    }

    public class PackageTargetFrameworks
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@container")]
        public string container { get; set; }
    }

    public class Parent
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }
    }

    public class Published
    {
        [JsonProperty("@type")]
        public string type { get; set; }
    }

    public class Reasons
    {
        [JsonProperty("@container")]
        public string container { get; set; }
    }

    public class Registration
    {
        [JsonProperty("@type")]
        public string type { get; set; }
    }

    public class NuGetPackageInfo
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public List<string> type { get; set; }
        public string commitId { get; set; }
        public DateTime commitTimeStamp { get; set; }
        public int count { get; set; }
        public List<Item> items { get; set; }

    }

    public class Tags
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@container")]
        public string container { get; set; }
    }


}
