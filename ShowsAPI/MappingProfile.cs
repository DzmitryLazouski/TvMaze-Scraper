using AutoMapper;
using Scraper.Data.Entities;
using Scraper.Data.Models;

namespace ShowsAPI
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ShowModel, Show>().ReverseMap();
            CreateMap<PersonModel, Person>().ReverseMap();
        }
    }
}
