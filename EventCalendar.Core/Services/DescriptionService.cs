using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Newtonsoft.Json;
using EventCalendar.Core.Enums;
using EventCalendar.Core.Dto;
using AutoMapper;

namespace EventCalendar.Core.Services
{
    public static class DescriptionService
    {
        public static EventDescription CreateDescription(EventDescription description)
        {
            ApplicationContext.Current.DatabaseContext.Database.Save(description);
            return description;
        }

        public static EventDescription UpdateDescription(EventDescription description)
        {
            ApplicationContext.Current.DatabaseContext.Database.Update(description);
            return description;
        }

        public static int DeleteDescription(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Delete<EventDescription>(id);
        }

        public static EventDescription GetDescriptionById(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_eventdescriptions").Where<EventDescription>(x => x.Id == id);

            return db.Fetch<EventDescription>(query).FirstOrDefault();
        }

        public static IEnumerable<EventDescription> GetAllDescriptions()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_eventdescriptions");

            return db.Fetch<EventDescription>(query);
        }

        public static IEnumerable<EventDescription> GetDescriptionsForEvent(int calendar, int @event, EventType type)
        {
            var query2 = new Sql().Select("*").From("ec_eventdescriptions").Where<EventDescription>(x => x.CalendarId == calendar && x.EventId == @event && x.Type == (int)type);
            return ApplicationContext.Current.DatabaseContext.Database.Fetch<EventDescription>(query2);
        }

        public static IEnumerable<Description> GetDescriptions(int parent, DescriptionType type)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From("ec_descriptions").Where<DescriptionDto>(x => x.ParentId == parent && x.DescriptionType == (int)type);
            return Mapper.Map<IEnumerable<Description>>(db.Fetch<DescriptionDto>(query));
        }

        public static object GetRTEConfiguration()
        {
            var query = new Sql().Select("value").From("cmsDataTypePreValues").Where("datatypeNodeId = -87");
            var tmp = ApplicationContext.Current.DatabaseContext.Database.FirstOrDefault<string>(query);
            if(!tmp.StartsWith("{")){
                return tmp;
            }
            return JsonConvert.DeserializeObject(tmp);
        }
    }
}
