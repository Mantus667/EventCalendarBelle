using AutoMapper;
using EventCalendar.Core.Dto;
using EventCalendar.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventCalendar.Core.Extensions;

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

                if (source.MediaItems.AnySave())
                {
                    dto.media = String.Join(",", source.MediaItems.ToArray());
                }

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

                result.MediaItems = new List<int>();
                if (!String.IsNullOrEmpty(source.media))
                {
                    result.MediaItems = source.media.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();
                }

                return result;
            }
        }
    }
}
