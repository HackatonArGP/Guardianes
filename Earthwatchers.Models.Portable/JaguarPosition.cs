using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earthwatchers.Models
{
    public class JaguarGame
    {
        public const int gameDuration = 3; //hours

        public int Id { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string FoundBy { get; set; }

        public bool IsAvailable()
        {
            var now = DateTime.Now;
            int today = (int)now.DayOfWeek;

            if (this.Day == today && this.FoundBy == null)
            {
                if (now.Hour >= this.Hour && now.Hour <= (this.Hour + gameDuration))
                {
                    return true;
                }
            }

            return false;
        }
        public DateTime GetFinalizationTime()
        {
            var now = DateTime.Now;
            var stDate = new DateTime(now.Year, now.Month, now.Day, Hour, Minutes, 0);
            var endDate = stDate.AddHours(gameDuration);

            return endDate;
        }
    }
}
