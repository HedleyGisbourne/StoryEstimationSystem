using AutoMapper;
using HG.SoftwareEstimationService.Dto;
using HG.SoftwareEstimationService.Dto.ViewModels;
using HG.SoftwareEstimationService.SqliteData;

namespace HG.SoftwareEstimationService.Services.Automapper
{
    public static class AutomapperRegistrar
    {
        private static readonly MapperConfiguration Automapper;
        
        static AutomapperRegistrar()
        {
            Automapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SystemInDevelopment, SystemInDevelopmentHomeGrid>();

                cfg.CreateMap<SystemInDevelopment, SystemInDevelopmentGrid>();
                cfg.CreateMap<SystemInDevelopmentGrid, SystemInDevelopment>();

                cfg.CreateMap<Story, StoryGrid>();
                cfg.CreateMap<StoryGrid, Story>();
               
                cfg.CreateMap<StoryPart, ObservationsGrid>()
                    .ForMember(d => d.PartType, opt => opt.MapFrom(src => src.PartType.Name));
                cfg.CreateMap<ObservationsGrid, StoryPart>();

                cfg.CreateMap<Property, PropertyDto>();
            });

        }

        public static TS Map<TS>(object source)
        {
            IMapper mapper = Automapper.CreateMapper();
            return mapper.Map<TS>(source);
        }
    }
}
