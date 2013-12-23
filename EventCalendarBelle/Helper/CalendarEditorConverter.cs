using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using System.Web.Script.Serialization;
using EventCalendarBelle.Models;

namespace EventCalendarBelle.Helper
{
    public class ECalendarPropertyEditorValueConverter : IPropertyEditorValueConverter
    {
        public Umbraco.Core.Attempt<object> ConvertPropertyValue(object sourceValue)
        {
            try
            {
                JavaScriptSerializer lizer = new JavaScriptSerializer();
                ECalendar cal = lizer.Deserialize<ECalendar>(sourceValue.ToString());
                return Attempt<object>.Succeed(cal);  //(true, cal);
            }
            catch { return Attempt<object>.Fail(); }
        }

        //public object ConvertDataToSource(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, object source, bool preview)
        //{
        //    throw new NotImplementedException();
        //}

        //public object ConvertSourceToObject(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, object source, bool preview)
        //{
        //    try
        //    {
        //        JavaScriptSerializer lizer = new JavaScriptSerializer();
        //        ECalendar cal = lizer.Deserialize<ECalendar>(source.ToString());
        //        return Attempt<object>.Succeed(cal);
        //    }
        //    catch { return Attempt<object>.Fail(); }
        //}

        //public object ConvertSourceToXPath(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType, object source, bool preview)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsConverter(Umbraco.Core.Models.PublishedContent.PublishedPropertyType propertyType)
        //{
        //    return propertyType.PropertyTypeAlias == "EventCalendar.CalendarPicker" ? true : false;
        //}

        //public Attempt<object> ConvertPropertyValue(object sourceValue)
        //{
        //    throw new NotImplementedException();
        //}

        public bool IsConverterFor(Guid datatypeGuid, string contentTypeAlias, string propertyTypeAlias)
        {
            return Guid.Parse("5eb3ed92-d489-4112-9900-1a239fb00751").Equals(datatypeGuid);
        }
    }
}
