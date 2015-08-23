using Umbraco.Core;
using AutoMapper;
using EventCalendar.Core.AutoMapperProfiles;

namespace EventCalendar.Core
{
    public class AutoMapperEvent : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Mapper.AddProfile<RecurringEventProfile>();
            Mapper.AddProfile<EventProfile>();
            Mapper.AddProfile<CalendarProfile>();
            Mapper.AddProfile<DateExceptionProfile>();
        }
    }
}
