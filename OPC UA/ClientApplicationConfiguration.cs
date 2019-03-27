using System.IO;
using System.Net;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace OPC
{
    /// <summary>
    /// Holds the application configuration for an OPC client
    /// </summary>
    public class ClientApplicationConfiguration
    {
        private static ApplicationConfiguration instance = null;

        /// <summary>
        /// Gets the application configuration instance
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="certificateStore">The file path to the certificate store</param>
        /// <returns>An application configuration to use for connections</returns>
        public static ApplicationConfiguration Get(string applicationName, string certificateStore, bool autoAccept)
        {

                if (instance == null)
                {
                    instance = new ApplicationConfiguration()
                    {
                        ApplicationName = applicationName,
                        ApplicationUri = $"urn:{ Dns.GetHostName()}:{applicationName}",
                        ApplicationType = ApplicationType.Client,
                        SecurityConfiguration = new SecurityConfiguration
                        {
                            ApplicationCertificate = new CertificateIdentifier { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(certificateStore, "Application"), SubjectName = $@"CN={applicationName}, O=CNC Data LTD, C=UK, DC={Dns.GetHostName()}" },
                            TrustedIssuerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(certificateStore, "TrustedIssuer") },
                            TrustedPeerCertificates = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(certificateStore, "TrustedPeers") },
                            RejectedCertificateStore = new CertificateTrustList { StoreType = CertificateStoreType.Directory, StorePath = Path.Combine(certificateStore, "RejectedCertificates") },
                            AutoAcceptUntrustedCertificates = autoAccept,
                            AddAppCertToTrustedStore = true
                        },
                        TransportConfigurations = new TransportConfigurationCollection(),
                        TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                        ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                        TraceConfiguration = new TraceConfiguration()
                    };

                    instance.Validate(ApplicationType.Client).GetAwaiter().GetResult();

                    // Check to see if client certificate has been created
                    var application = new ApplicationInstance
                    {
                        ApplicationName = applicationName,
                        ApplicationType = ApplicationType.Client,
                        ApplicationConfiguration = instance
                    };

                    application.CheckApplicationInstanceCertificate(false, 4096, 48).GetAwaiter().GetResult();
                
            }

            return instance;
        }
    }
}
