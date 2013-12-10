using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Core;

namespace EventCalendarBelle
{
    [Application("eventCalendar","EventCalendar","icon-calendar-alt")]
    public class EventCalendarApplication : IApplication
    {
        public static ECalendar GetCalendar(int id)
        {
            if (id == 0)
            {
                return null;
            }
            else
            {
                return ApplicationContext.Current.DatabaseContext.Database.SingleOrDefault<ECalendar>(id);
            }
        }
    }
}
