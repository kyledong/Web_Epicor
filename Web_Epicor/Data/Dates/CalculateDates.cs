using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Epicor.Data.Dates
{
    public class CalculateDates
    {
        public static Tuple<DateTime, DateTime> Monthly()
        {
            
            //DateTime today = DateTime.Now);
            DateTime today = new DateTime(2020, 06, 01); //temporal
            today = today.AddDays(-1);
            DateTime startDate = new DateTime(today.Year, today.Month, 1);
            DateTime finalDate;
            if (today.Month + 1 < 13)
            {
                finalDate = new DateTime(today.Year, today.Month + 1, 1).AddDays(-1);
            }
            else
            {
                finalDate = new DateTime(today.Year + 1, 1, 1).AddDays(-1);
            }
            return Tuple.Create(startDate, finalDate);
        }

        public static Tuple<DateTime, DateTime> Weekly()
        {
            
            DateTime startWeek;
            //DateTime finalWeek = DateTime.Now;
            DateTime finalWeek = new DateTime(2020, 09, 14); // temporal
            int delta = DayOfWeek.Monday - finalWeek.DayOfWeek;
            if (delta > 0)
            {
                delta -= 7;
                startWeek = finalWeek.AddDays(delta);
            }
            else
            {
                startWeek = finalWeek.AddDays(-7);
            }
            return Tuple.Create(startWeek, finalWeek);

        }
    }
}
