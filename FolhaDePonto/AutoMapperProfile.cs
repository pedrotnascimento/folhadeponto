using AutoMapper;
using Common;
using Domain.Entities;
using FolhaDePonto.DTO;

namespace FolhaDePonto
{
    public class AutoMapperProfiler : Profile
    {

        public AutoMapperProfiler()
        {
            CreateMap<TimeMomentCreateDTO, TimeMoment>()
            .ForMember(dest => dest.DateTime, map => map.MapFrom(x => x.DataHora));

            CreateMap<IEnumerable<TimeMoment>, RegisterDTO>()
                .ForMember(dest => dest.Horarios, map => map.MapFrom(x => x.Select(x => x.DateTime.ToLongTimeString())))
                .ForMember(dest => dest.Dia, map => map.MapFrom(x => x.FirstOrDefault().DateTime.Date.ToShortDateString()));

            CreateMap<AllocationCreateDTO, TimeAllocation>()
                .ForMember(dest => dest.TimeDuration, map => map.MapFrom(x => Duration.Parse(x.Tempo)))
                .ForMember(dest => dest.ProjectName, map => map.MapFrom(x => x.NomeProjeto))
                .ForMember(dest => dest.Date, map => map.MapFrom(x => x.Dia));

            CreateMap<TimeAllocation,AllocationCreateDTO>()
                .ForMember(dest => dest.Tempo, map => map.MapFrom(x => Duration.FromDateTime(x.TimeDuration)))
                .ForMember(dest => dest.NomeProjeto, map => map.MapFrom(x => x.ProjectName))
                .ForMember(dest => dest.Dia, map => map.MapFrom(x => x.Date));
        }

    }
}
