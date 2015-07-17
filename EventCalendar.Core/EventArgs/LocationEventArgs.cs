using System.ComponentModel;
using EventCalendar.Core.Models;

namespace EventCalendar.Core.EventArgs
{
    public class LocationCreatingEventArgs : CancelEventArgs
    {
        public EventLocation Location { get; set; }
    }

    public class LocationCreatedEventArgs : System.EventArgs
    {
        public EventLocation Location { get; set; }
    }

    public class LocationDeletionEventArgs : CancelEventArgs
    {
        public EventLocation Location { get; set; }
    }

    public class LocationDeletedEventArgs : System.EventArgs
    {
        public EventLocation Location { get; set; }
    }
}
