using AutoMapper;
using BusinessRule.Domain;
using Common;
using FolhaDePonto.DTO;

namespace FolhaDePonto.AutoMapper
{
    public class DTOtoBRProfileMapper : Profile
    {

        public DTOtoBRProfileMapper()
        {
            CreateMap<TimeMomentCreateDTO, TimeMomentBR>()
            .ForMember(dest => dest.DateTime, map => map.MapFrom(x => x.DataHora));

            CreateMap<IEnumerable<TimeMomentBR>, RegisterResponseDTO>()
                .ForMember(dest => dest.Horarios, map => map.MapFrom(x => x.Select(x => x.DateTime.ToLongTimeString())))
                .ForMember(dest => dest.Dia, map => map.MapFrom(x => x.FirstOrDefault().DateTime.Date.ToShortDateString()));

            CreateMap<AllocationCreateDTO, TimeAllocationBR>()
                .ForMember(dest => dest.TimeDuration, map => map.MapFrom(x => Duration.Parse(x.Tempo)))
                .ForMember(dest => dest.ProjectName, map => map.MapFrom(x => x.NomeProjeto))
                .ForMember(dest => dest.Date, map => map.MapFrom(x => x.Dia));

            CreateMap<TimeAllocationBR, AllocationCreateDTO>()
                .ForMember(dest => dest.Tempo, map => map.MapFrom(x => Duration.FromDateTime(x.TimeDuration)))
                .ForMember(dest => dest.NomeProjeto, map => map.MapFrom(x => x.ProjectName))
                .ForMember(dest => dest.Dia, map => map.MapFrom(x => x.Date));

            CreateMap<TimeAllocationBR, AllocationReportResponseDTO>()
                .ForMember(dest => dest.NomeProjeto, map => map.MapFrom(x => x.ProjectName))
                .ForMember(dest => dest.Tempo, map => map.MapFrom(x => Duration.FromDateTime(x.TimeDuration)));

            CreateMap<ReportDataBR, ReportResponseDTO>()
                .ForMember(dest => dest.Mes, map => map.MapFrom(x => x.Month.Date.ToShortDateString()))
                .ForMember(dest => dest.HorasTrabalhadas, map => map.MapFrom(x => Duration.FromDateTime(x.WorkedTime)))
                .ForMember(dest => dest.HorasExcendentes, map => map.MapFrom(x => Duration.FromDateTime(x.ExceededWorkedTime)))
                .ForMember(dest => dest.HorasDevidas, map => map.MapFrom(x => Duration.FromDateTime(x.DebtTime)));
        }

    }
}
