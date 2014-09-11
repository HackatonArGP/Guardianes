using System;

namespace Earthwatchers.Models
{
    public class StatisticsQuery
    {
        public DateTime Date { get; set; }
        public int? Data { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Stat { get; set; }

    }

    //public enum StatQueryName
    //{
    //    UnicLogin,
    //    TotalLogin,
    //    NewUsers,
    //    NewRegister,
    //    Shared,
    //    Denounce,
    //    Silverlight,
    //    TutorialCompletedOld,
    //    NewUserCheckOther,
    //    NewUserCheckOwn
    //}

}
