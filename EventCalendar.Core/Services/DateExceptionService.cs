using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace EventCalendar.Core.Services
{
    public static class DateExceptionService
    {
        public static int DeleteDateException(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            return db.Delete<DateExceptionDto>(id);
        }

        public static IEnumerable<DateException> GetDateExceptionsForRecurringEvent(int eventId)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From<DateExceptionDto>().Where<DateExceptionDto>(x => x.EventId == eventId);

            var results = db.Fetch<DateExceptionDto>(query);

            return Mapper.Map<IEnumerable<DateException>>(results);
        }

        public static DateException GetDateException(int id)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var query = new Sql().Select("*").From<DateExceptionDto>().Where<DateExceptionDto>(x => x.Id == id);
            var result = db.First<DateExceptionDto>(query);

            return Mapper.Map<DateException>(result);
        }

        public static DateException UpdateDateException(DateException de)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var dto = Mapper.Map<DateExceptionDto>(de);

            db.Update(dto);

            return Mapper.Map<DateException>(dto);
        }

        public static DateException CreateDateException(DateException de)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;
            var dto = Mapper.Map<DateExceptionDto>(de);

            db.Insert(dto);

            return Mapper.Map<DateException>(dto);
        }
    }
}
