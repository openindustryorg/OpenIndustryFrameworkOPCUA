using Models;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OPC
{
    public class ClientSubscription
    {
        //This is generic EventHandler delegate where 
        //we define the type of argument want to send 
        //while raising our event, NotificationEventArgs in our case.
        public event EventHandler<NotificationEventArgs> OnNotificationEventHandler = delegate { };

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int publishingIntervalSeconds = 1000;

        private Session opcSession;
        

        /// <summary>
        /// Creates a new Client
        /// </summary>
        /// <param name="ApplicationName">The OPC Application Name to register</param>
        /// <param name="EndpointURL">The OPC endpoint </param>
        /// <param name="_autoAccept">Accept any certificate regardless</param>
        public ClientSubscription(Session OpcSession)
        {
            opcSession = OpcSession;
        }

        public Subscription Execute(List<DataItem> dataItems)
        {
            Subscription subscription = new Subscription(opcSession.DefaultSubscription) {
                PublishingInterval = publishingIntervalSeconds
            };

            List<MonitoredItem> monitoredItems = CreateReadValueIdCollection(dataItems);

            monitoredItems.ForEach(i => i.Notification += OnNotification);

            subscription.AddItems(monitoredItems);

            opcSession.AddSubscription(subscription);

            subscription.Create();

            return subscription;
        }

        private static List<MonitoredItem> CreateReadValueIdCollection(IEnumerable<DataItem> opcItems)
        {
            var monitoredItems = new List<MonitoredItem>();

            foreach (DataItem item in opcItems)
            {
                monitoredItems.Add(new MonitoredItem()
                {
                    DisplayName = item.Description,
                    StartNodeId = item.Tag
                });
            };

            return monitoredItems;
        }

        /// <summary>
        /// OPC Item callback
        /// </summary>
        /// <param name="item"></param>
        /// <param name="e"></param>
        private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            //Initialize NotificationEventArgs OPCItemDTO with MonitoredItem data
            foreach (var value in item.DequeueValues())
            {
                OPC.Helpers.NotificationEventArgs notificationEventArgs = new OPC.Helpers.NotificationEventArgs(new NotificationEvent
                {
                    Tag = item.ResolvedNodeId.ToString()
                });

                //Create list of exception
                List<Exception> exceptions = new List<Exception>();

                //Invoke OnChange Action by iterating on all subscribers event handlers
                foreach (Delegate handler in OnNotificationEventHandler.GetInvocationList())
                {
                    try
                    {
                        //pass sender object and eventArgs while
                        handler.DynamicInvoke(this, notificationEventArgs);
                    }
                    catch (Exception ex)
                    {
                        //Add exception in exception list if occured any
                        exceptions.Add(ex);
                    }
                }

                //Check if any exception occured while 
                //invoking the subscribers event handlers
                if (exceptions.Any())
                {
                    //Throw aggregate exception of all exceptions 
                    //occured while invoking subscribers event handlers
                    throw new AggregateException(exceptions);
                }
            }
        }


    }
}
