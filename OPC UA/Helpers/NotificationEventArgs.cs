using Models;

namespace OPC.Helpers
{
    public class NotificationEventArgs
    {
        public NotificationEvent Value { get; set; }

        public NotificationEventArgs(NotificationEvent opcItem)
        {
            Value = opcItem;
        }
    }
}
