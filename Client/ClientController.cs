using Models;
using Opc.Ua.Client;
using OPC;
using System;
using System.Collections.Generic;

namespace OPCUAClient
{
    public class ClientController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string applicationName;
        private string endpointURL;
        private List<DataItem> opcItems;
        private bool autoAccept;

        private Session opcSession;

        /// <summary>
        /// Client Manager Class Initaliser
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="EndpointURL"></param>
        /// <param name="OPCItems"></param>
        /// <param name="AutoAccept"></param>
        public ClientController(string ApplicationName, string EndpointURL, List<DataItem> OPCItems, bool AutoAccept)
        {
            opcItems = OPCItems;
            applicationName = ApplicationName;
            endpointURL = EndpointURL;
            autoAccept = AutoAccept;

            opcSession = new ClientSession(applicationName, endpointURL, autoAccept).Execute();

            ClientSubscription clientSubscription = new ClientSubscription(opcSession);
            clientSubscription.OnNotificationEventHandler += (sender, e) => OnClientSubscriptionNotificationEventHandler(sender, e);
            clientSubscription.Execute(opcItems);
        }

        public void Execute()
        {
            try
            {
                //Check we have an active OPCUa Session, attempt to create if session is not active
                if (opcSession == null || opcSession.KeepAliveStopped == true)
                {
                    opcSession = new ClientSession(applicationName, endpointURL, autoAccept).Execute();
                }

                //Read OPC DataItems
                var newDataValueCollection = ClientReadDataItemValues.Execute(opcItems, opcSession);
            }
            catch (Exception ex)
            {
                log.Error($"Client Controller Execute Exception:", ex);
            }
        }

        /// <summary>
        /// Sets the manual reset event to cancel the Start methods waitInfinite
        /// </summary>
        public void Stop()
        {
            try
            {
                // disconnect any existing session.
                if (opcSession != null)
                {
                    opcSession.Close(10000);
                    opcSession = null;
                }

                log.Info($"OPC Session Stopped");
            }
            catch (Exception ex)
            {
                log.Error($"Client Controller Stop Exception", ex);
            }
        }

        private void OnClientSubscriptionNotificationEventHandler(object sender, NotificationEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
