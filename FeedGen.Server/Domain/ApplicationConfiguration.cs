using System;

namespace FeedGen.Server.Domain {
    public sealed class ApplicationConfiguration {
        public string RootDirectory { get; }
        public string ExternalAddress { get; }
        public ApplicationConfiguration( string rootDirectory, string externalAddress ) {
            if (string.IsNullOrWhiteSpace( rootDirectory )) {
                throw new ArgumentNullException( "A root directory was not configured", nameof( rootDirectory ) );
            }

            if (string.IsNullOrWhiteSpace( externalAddress )) {
                throw new ArgumentNullException( "An external address was not configured", nameof( externalAddress ) );
            }

            RootDirectory = rootDirectory;
            ExternalAddress = externalAddress;
        }
    }
}
