using System;
using System.Data.Services.Client;
using System.Linq;

namespace NuGet {
    public class DataServicePackageRepository : PackageRepositoryBase {
        private readonly IDataServiceContext _context;
        private readonly IHttpClient _httpClient;
 
        public DataServicePackageRepository(Uri serviceRoot, IHttpClient client)
            : this(new DataServiceContextWrapper(serviceRoot), client) {
        }

        public DataServicePackageRepository(IDataServiceContext context, IHttpClient httpClient) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            _context = context;
            _httpClient = httpClient;

            _context.SendingRequest += OnSendingRequest;
            _context.ReadingEntity += OnReadingEntity;
            _context.IgnoreMissingProperties = true;
        }

        private void OnReadingEntity(object sender, ReadingWritingEntityEventArgs e) {
            var package = (DataServicePackage)e.Entity;

            // REVIEW: This is the only way (I know) to download the package on demand
            // GetReadStreamUri cannot be evaluated inside of OnReadingEntity. Lazily evaluate it inside DownloadPackage
            package.Context = _context;
        }

        private void OnSendingRequest(object sender, SendingRequestEventArgs e) {
            // Initialize the request
            _httpClient.InitializeRequest(e.Request);
        }

        public override IQueryable<IPackage> GetPackages() {
            // REVIEW: Is it ok to assume that the package entity set is called packages?
            return new SmartDataServiceQuery<DataServicePackage>(_context, Constants.PackageServiceEntitySetName).AsSafeQueryable();
        }
    }
}
