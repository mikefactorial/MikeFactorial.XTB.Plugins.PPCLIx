﻿using Microsoft.Crm.Sdk.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Hosting;

namespace MikeFactorial.XTB.PPCLIx
{
    public class PacCLINugetFeed
    {
        private HttpClient client = new HttpClient();

        public string PackageId => "Microsoft.PowerApps.CLI";

        public HttpClient Client
        {
            get
            {
                return client;
            }
            set
            {
                client = value;
            }
        }
        public async virtual Task<Stream> DownloadPackageStreamAsync(string version)
        {
            return await Client.GetStreamAsync($"https://www.nuget.org/api/v2/package/{PackageId}/{version}");
        }

        public async virtual Task<Stream> DownloadPackageInfoStreamAsync()
        {
            return await Client.GetStreamAsync($"https://api.nuget.org/v3/registration5-semver1/{PackageId.ToLower()}/index.json");
        }
        public async virtual Task Initialize()
        {
            using (var stream = await DownloadPackageInfoStreamAsync())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var json = reader.ReadToEnd();
                    NuGetPackageInfo packageInfo = JsonConvert.DeserializeObject<NuGetPackageInfo>(json);

                    this.Versions = new List<string>();
                    foreach (var item in packageInfo.items)
                    {
                        foreach (var item2 in item.items)
                        {
                            this.Versions.Add(item2.catalogEntry.version);
                        }
                    }
                }
            }
            Versions.Reverse();
        }

        public virtual List<string> Versions
        { 
            get; set; 
        }

        public virtual string NugetVersionLoaded 
        { 
            get; set; 
        }
    }
}
