using System.ComponentModel;
using EventCalendar.Core.Models;

namespace EventCalendar.Core.EventArgs
{
    public class RecurringEventCreatingEventArgs : CancelEventArgs
    {
        public RecurringEvent RecurringEvent { get; set; }
    }

    public class RecurringEventCreatedEventArgs : System.EventArgs
    {
        public RecurringEvent RecurringEvent { get; set; }
    }
}
