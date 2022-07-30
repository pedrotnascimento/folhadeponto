using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DataAccessLayer
{
    public class TimeMomentDAL
    {
        public int UserId { get; set; }
        public UserDAL User { get; set; }
        public DateTime DateTime { get; set; }
    }
}
