using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
