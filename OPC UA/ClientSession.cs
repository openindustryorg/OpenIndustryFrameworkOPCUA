using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Threading.Tasks;

namespace OPC
{
    public class ClientSession
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        const int selectEndpointOperationTimeoutSeconds = 15 * 1000;
        const int sessionTimeoutSeconds = 60 * 1000;
        const int reconnectPeriodSeconds = 10 * 1000;

        private Session session = null;
        private SessionReconnectHandler reconnectHandler;

        private static string endpointURL;
        private static bool autoAccept = false;
        private static string applicationName;

        /// <summary>
        /// Creates a new Client
        /// </summary>
        /// <param name="ApplicationName">The OPC Application Name to register</param>
        /// <param name="EndpointURL">The OPC endpoint </param>
        /// <param name="_autoAccept">Accept any certificate regardless</param>
        public ClientSession(string ApplicationName, string EndpointURL, bool AutoAccept)
        {
            applicationName = ApplicationName;
            endpointURL = EndpointURL;
            autoAccept = AutoAccept;
        }

        public Session Execute()
        {
            try
            {
                log.Info("Create a OPC UA session.");

                session = GetSession().Result;
            }
            catch (Exception ex)
            {
                log.Error("Create a OPC UA session Error", ex);
            }

            return session;
        }

        /// <summary>
        /// Creates a Session
        /// </summary>
        /// <returns>Async Task</returns>
        private async Task<Session> GetSession()
        {
            var securityConfiguration = new ClientSecurityConfiguration(MessageSecurityMode.SignAndEncrypt);

            var applicationConfiguration = ClientApplicationConfiguration.Get(
                    applicationName,
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CNC Data",
                    autoAccept);

            var endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);

            var endpointDescription = CoreClientUtils.SelectEndpoint(endpointURL, true, selectEndpointOperationTimeoutSeconds );

            var configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            var session = await Session.Create(applicationConfiguration, configuredEndpoint, false, applicationName, sessionTimeoutSeconds, securityConfiguration.UserIdentity(), null);

            session.KeepAlive += OnKeepAliveEvent;

            return session;
        }

        /// <summary>
        /// The created Session KeepAlive Callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeepAliveEvent(Session session, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                log.Warn($"KeepAlive {e.Status} {session.OutstandingRequestCount} {session.DefunctRequestCount}");

                if (reconnectHandler == null)
                {
                    log.Info("KeepAlive attempting Reconnect");
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(session, reconnectPeriodSeconds, OnKeepAliveComplete);
                }
            }
        }

        /// <summary>
        /// The KeepAlive complete callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeepAliveComplete(object sender, EventArgs e)
        {
            //Ignore callbacks from discarded objects.
            if (!Object.ReferenceEquals(sender, reconnectHandler))
            {
                return;
            }

            session = reconnectHandler.Session;

            reconnectHandler.Dispose();
            reconnectHandler = null;

            log.Info("Session KeepAlive Reconnected");
        }
    }
}
