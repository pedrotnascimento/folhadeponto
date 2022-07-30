using AutoMapper;
using Repository.DataAccessLayer;
using Repository.Tables;

namespace FolhaDePonto.AutoMapper
{
    public class DALtoTableProfileMapper : Profile
    {

        public DALtoTableProfileMapper()
        {
            CreateMap<TimeMomentDAL, TimeMoment>().ReverseMap();
            CreateMap<TimeAllocationDAL, TimeAllocation>().ReverseMap();
        }

    }
}
