using System.Data;
using AutoMapper;
using AutoMapper.Data;
using Models;

namespace Data.AutoMapper
{
    public static class Mapper
    {
        private static MapperConfiguration mapperConfig;
        public static IMapper mapper;

        public static void initMapper() 
        {
            mapperConfig = new MapperConfiguration(cfg => 
            {
                cfg.AddDataReaderMapping();
                cfg.CreateMap<IDataRecord, Case>()
                    .ForPath(dest => dest.Id, opt => opt.MapFrom(src => src.GetInt32(0)))
                    .ForPath(dest => dest.Kind.Id, opt => opt.MapFrom(src => src.GetInt32(5)))
                    .ForPath(dest => dest.Kind.Type, opt => opt.MapFrom(src => src.GetString(6)))
                    .ForPath(dest => dest.Kind.Description, opt => opt.MapFrom(src => src.GetString(7)));
                cfg.CreateMap<IDataRecord, CaseKind>();
            });
            mapper = mapperConfig.CreateMapper();
        }
    }
}