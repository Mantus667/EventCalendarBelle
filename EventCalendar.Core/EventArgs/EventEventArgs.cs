using System.ComponentModel;

namespace EventCalendar.Core.EventArgs
{
    public class EventCreatingEventArgs : CancelEventArgs
    {
        public Models.Event @Event { get; set; }
    }

    public class EventCreatedEventArgs : System.EventArgs
    {
        public Models.Event @Event { get; set; }
    }
}
