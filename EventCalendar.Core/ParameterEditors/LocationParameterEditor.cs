using EventCalendar.Core.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.PropertyEditors;

namespace EventCalendar.Core.ParameterEditors
{
    /// <summary>
    /// Property Editor for Locations used only for macros
    /// </summary>
    [PropertyEditor("EventCalendar.LocationMacroDropdown", "EventCalendar Location MacroDropDown", "dropdown", IsParameterEditor = true, ValueType = "int")]
    public class LocationParameterEditor : PropertyEditor
    {
        public LocationParameterEditor()
        {
            this.DefaultPreValues = (IDictionary<string, object>)new Dictionary<string, object>();
            var locations = LocationService.GetAllLocations().ToDictionary(x => x.Id, x => x.LocationName);
            this.DefaultPreValues.Add("items", (object)locations);
        }
    }
}
