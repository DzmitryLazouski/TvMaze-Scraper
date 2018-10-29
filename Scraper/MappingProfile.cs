using AutoMapper;
using Scraper.Data.DTO;
using Scraper.Data.Models;

namespace Scraper
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ShowModel, ShowDto>();
            CreateMap<PersonModel, PersonDto>();
        }
    }
}
