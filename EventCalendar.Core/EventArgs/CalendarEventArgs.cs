using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCalendar.Core.Models;

namespace EventCalendar.Core.EventArgs
{
    public class CalendarCreationEventArgs : CancelEventArgs
    {
        public ECalendar Calendar { get; set; }
    }

    public class CalendarCreatedEventArgs : System.EventArgs
    {
        public ECalendar Calendar { get; set; }
    }
    public class CalendarDeletionEventArgs : CancelEventArgs
    {
        public ECalendar Calendar { get; set; }
    }

    public class CalendarDeletedEventArgs : System.EventArgs
    {
        public ECalendar Calendar { get; set; }
    }

}
