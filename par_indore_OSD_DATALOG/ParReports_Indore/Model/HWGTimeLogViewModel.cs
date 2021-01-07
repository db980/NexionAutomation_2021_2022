using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParReports_Indore.Model
{
    public class ReportViewModel:ICloneable
    {
        public DateTime DateAndTime { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PLCTag { get; set; }
        public string Description { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
