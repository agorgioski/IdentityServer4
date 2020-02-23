using AutoMapper;
using Models;

namespace Web.AutoMapper
{
    public static class Mapper
    {
        private static MapperConfiguration mapperConfig;
        public static IMapper mapper;

        public static void initMapper() 
        {
            mapperConfig = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<Case, CaseModel>()
                    .ForPath(dest => dest.Kind, opt => opt.MapFrom(src => src.Kind.Type));
                cfg.CreateMap<CaseModel, Case>()
                    .ForPath(dest => dest.Kind, opt => opt.MapFrom(src => src.Kind));
                cfg.CreateMap<string, CaseKind>();
            });
            mapper = mapperConfig.CreateMapper();
        }
    }
}