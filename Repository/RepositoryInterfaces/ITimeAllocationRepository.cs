using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.RepositoryInterfaces
{
    public interface ITimeAllocationRepository
    {
        void Create(TimeAllocation timeAlocation);
        TimeAllocation? GetByDate(DateTime dateTime);
        void Update(TimeAllocation timeAlocation);
    }
}
