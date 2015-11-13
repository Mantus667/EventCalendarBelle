using EventCalendar.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.PropertyEditors;

namespace EventCalendar.Core.ParameterEditors
{
    [PropertyEditor("EventCalendar.CalendarMacroDropdown", "EventCalendar MacroDropDown", "dropdown", IsParameterEditor = true, ValueType = "int")]
    public class CalendarParemeterEditor : PropertyEditor
    {
        public CalendarParemeterEditor()
        {
            this.DefaultPreValues = (IDictionary<string, object>)new Dictionary<string, object>();
            var calendar = CalendarService.GetAllCalendar().ToDictionary(x => x.Id, x => x.Calendarname);
            this.DefaultPreValues.Add("items", (object)calendar);
        }
    }
}
