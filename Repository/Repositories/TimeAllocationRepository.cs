using AutoMapper;
using Repository.DataAccessLayer;
using Repository.RepositoryInterfaces;
using Repository.Tables;

namespace Repository.Repositories
{
    public class TimeAllocationRepository : ITimeAllocationRepository
    {
        private readonly FolhaDePontoContext context;
        private readonly IMapper mapper;

        public TimeAllocationRepository(FolhaDePontoContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public TimeAllocationDAL? GetByDate(DateTime dateTime)
        {
            var result = this.context.TimeAllocations.FirstOrDefault(x => x.Date == dateTime);
            var ret= mapper.Map<TimeAllocation, TimeAllocationDAL>(result);
            return ret;
        }

        public void Create(TimeAllocationDAL timeAlocation)
        {
            var instance = mapper.Map<TimeAllocationDAL, TimeAllocation>(timeAlocation);
            this.context.TimeAllocations.Add(instance);
            context.SaveChanges();
        }

        public void Update(TimeAllocationDAL timeAlocation)
        {
            var instance = mapper.Map<TimeAllocationDAL, TimeAllocation>(timeAlocation);
            this.context.TimeAllocations.Update(instance);

            context.SaveChanges();
        }

    }
}
