using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace EventCalendarBelle
{
    public class EventCalendarHelper
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
