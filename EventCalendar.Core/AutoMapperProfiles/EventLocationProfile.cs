using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendar.Core.AutoMapperProfiles
{
    public class EventLocationProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<EventLocation, EventLocationDto>().ConvertUsing<EventLocationDtoConverter>();
            Mapper.CreateMap<EventLocationDto, EventLocation>().ConvertUsing<EventLocationConverter>();
        }

        private class EventLocationDtoConverter : ITypeConverter<EventLocation, EventLocationDto>
        {
            public EventLocationDto Convert(ResolutionContext context)
            {
                var source = (EventLocation)context.SourceValue;
                var dto = new EventLocationDto
                {
                    Id = source.Id,
                    City = source.City,
                    Country = source.Country,
                    Latitude = source.Latitude,
                    LocationName = source.LocationName,
                    Longitude = source.Longitude,
                    Street = source.Street,
                    ZipCode = source.ZipCode
                };
                return dto;
            }
        }

        private class EventLocationConverter : ITypeConverter<EventLocationDto, EventLocation>
        {
            public EventLocation Convert(ResolutionContext context)
            {
                var source = (EventLocationDto)context.SourceValue;
                var result = new EventLocation
                {
                    Id = source.Id,
                    City = source.City,
                    Country = source.Country,
                    Latitude = source.Latitude,
                    LocationName = source.LocationName,
                    Longitude = source.Longitude,
                    Street = source.Street,
                    ZipCode = source.ZipCode
                };

                return result;
            }
        }
    }
}
