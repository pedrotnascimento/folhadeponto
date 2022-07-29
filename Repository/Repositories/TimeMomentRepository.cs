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
    public class TimeMomentRepository : ITimeMomentRepository
    {
        private readonly FolhaDePontoContext context;

        public TimeMomentRepository(FolhaDePontoContext context)
        {
            this.context = context;
        }

        public void Create(TimeMoment timeMoment)
        {
            this.context.TimeMoments.Add(timeMoment);
            context.SaveChanges();
        }

        public IList<TimeMoment> QueryByUserIdAndDate(int userId, DateTime date)
        {
            var query = this.context.TimeMoments.Where(x => x.UserId == userId && x.DateTime.Date==date);
            return query.ToList();
        }
    }
}
