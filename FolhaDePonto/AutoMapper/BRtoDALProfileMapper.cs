using AutoMapper;
using BusinessRule.Domain;
using Common;
using FolhaDePonto.DTO;
using Repository.DataAccessLayer;

namespace FolhaDePonto.AutoMapper
{
    public class BRtoDALProfileMapper : Profile
    {

        public BRtoDALProfileMapper()
        {
            CreateMap<TimeMomentBR, TimeMomentDAL>().ReverseMap();
            CreateMap<TimeAllocationBR, TimeAllocationDAL>().ReverseMap();
            CreateMap<UserBR, UserDAL>().ReverseMap();
        }

    }
}
