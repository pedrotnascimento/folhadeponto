using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class TimeAllocationRepository : ITimeAllocationRepository
    {
        private readonly FolhaDePontoContext context;

        public TimeAllocationRepository(FolhaDePontoContext context)
        {
            this.context = context;
        }

        public TimeAllocation? GetByDate(DateTime dateTime)
        {
            var ret = this.context.TimeAllocations.FirstOrDefault(x => x.Date == dateTime);
            return ret;
        }

        public void Create(TimeAllocation timeAlocation)
        {
            this.context.TimeAllocations.Add(timeAlocation);
            context.SaveChanges();
        }

        public void Update(TimeAllocation timeAlocation)
        {

            this.context.TimeAllocations.Update(timeAlocation);

            context.SaveChanges();
        }

    }
}
