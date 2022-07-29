using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.RepositoryInterfaces
{
    public interface ITimeMomentRepository
    {
        void Create(TimeMoment timeMoment);
        IList<TimeMoment> QueryByUserIdAndDate(int userId, DateTime date);
    }
}
