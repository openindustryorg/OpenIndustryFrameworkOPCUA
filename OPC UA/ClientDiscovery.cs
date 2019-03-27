using System;
using System.Collections;

using Opc.Ua;
using Opc.Ua.Client;

namespace Siemens.OpcUA
{
    public class Discovery
    {
        #region Construction
        public Discovery()
        {
        }
        #endregion

        #region Public Interfaces
        /// <summary>
        /// Returns a list of OPC UA servers.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="servers">The servers being found.</param>
        public void FindServers(Uri discoveryUrl, ref ApplicationDescriptionCollection servers)
        {
            try
            {
                // Create Discovery Client.
                DiscoveryClient client = DiscoveryClient.Create(discoveryUrl);

                // Look for servers.
                servers = client.FindServers(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns a list of endpoints for a server.
        /// </summary>
        /// <param name="discoveryUrl">The discovery URL.</param>
        /// <param name="endpoints">The according endpoints.</param>
        public void GetEndpoints(Uri discoveryUrl, ref EndpointDescriptionCollection endpoints)
        {
            try
            {
                // Create Discovery Client.
                DiscoveryClient client = DiscoveryClient.Create(discoveryUrl);

                // Get endpoints.
                endpoints = client.GetEndpoints(null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}