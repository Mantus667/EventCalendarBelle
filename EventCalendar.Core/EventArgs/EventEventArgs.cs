using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
