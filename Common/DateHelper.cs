using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class DateHelper

    {
        public static int HoursInAMonthCountedByFixedHours(DateTime month, int hoursFixed)
        {
            var workDays = WorkDaysInAMonth(month);
            var totalHoursDemanded = hoursFixed * workDays;
            return totalHoursDemanded;
        }

        public static int WorkDaysInAMonth(DateTime month)
        {
            var dateTimeJustDate = month.Date;
            var iterationDay = new DateTime(dateTimeJustDate.Year, dateTimeJustDate.Month, 1);
            var workDays = 0;
            while (iterationDay.Month == dateTimeJustDate.Month)
            {
                if (iterationDay.DayOfWeek != DayOfWeek.Saturday && iterationDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    workDays++;
                }
                iterationDay = iterationDay.AddDays(1);
            }
            return workDays;
        }
    }

}
