using Microsoft.Crm.Sdk.Messages;
using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MikeFactorial.XTB.PACUI
{
    public class PacCLINugetFeed
    {
        private SourceCacheContext cache = new SourceCacheContext();
        private SourceRepository repository = NuGet.Protocol.Core.Types.Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");

        public NuGet.Common.ILogger Logger => NullLogger.Instance;

        public CancellationToken CancellationToken => CancellationToken.None;

        public string PackageId => "Microsoft.PowerApps.CLI";

        public async virtual Task Initialize()
        {
            this.Resource = await Repository.GetResourceAsync<FindPackageByIdResource>();
            this.Versions = await Resource.GetAllVersionsAsync(PackageId, Cache, Logger, CancellationToken);
        }
        public virtual SourceCacheContext Cache
        {
            get
            {
                return cache;
            }
        }
        public virtual SourceRepository Repository
        {
            get
            {
                return repository;
            }
        }

        public virtual FindPackageByIdResource Resource
        {
            get; set;
        }
        public virtual IEnumerable<NuGetVersion> Versions
        { 
            get; set; 
        }

        public virtual string NugetVersionLoaded 
        { 
            get; set; 
        }
    }
}
