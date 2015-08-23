using AutoMapper;
using EventCalendar.Core.Models;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.AutoMapperProfiles
{
    public class DateExceptionProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<DateException, DateExceptionDto>().ConvertUsing<DateExceptionDtoConverter>();
            Mapper.CreateMap<DateExceptionDto, DateException>().ConvertUsing<DateExceptionConverter>();
        }

        private class DateExceptionDtoConverter : ITypeConverter<DateException, DateExceptionDto>
        {
            public DateExceptionDto Convert(ResolutionContext context)
            {
                var source = (DateException)context.SourceValue;
                var result = new DateExceptionDto
                {
                    Id = source.Id,
                    EventId = source.EventId,
                    Date = source.Date
                };
                return result;
            }
        }
        private class DateExceptionConverter : ITypeConverter<DateExceptionDto, DateException>
        {
            public DateException Convert(ResolutionContext context)
            {
                var source = (DateExceptionDto)context.SourceValue;
                var result = new DateException
                {
                    Id = source.Id,
                    EventId = source.EventId,
                    Date = source.Date
                };
                return result;
            }
        }
    }
}
