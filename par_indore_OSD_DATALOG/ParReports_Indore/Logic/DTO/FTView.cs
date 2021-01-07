using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParReports_Indore
{
    public static partial class DataReportDBStoredProcedures
    {
        public static IEnumerable<ReportingDataInput> GetReportDataInput(this DataConnection dataConnection, DateTime? @StartDateTime, DateTime? @EndDateTime)
        {

            return dataConnection.QueryProc<ReportingDataInput>("[NexionAutomation].[GetReportDataInput]",
                new DataParameter("@StartDateTime", @StartDateTime, LinqToDB.DataType.DateTime),
                new DataParameter("@EndDateTime", @EndDateTime, LinqToDB.DataType.DateTime));
        }
    }
            
    public class ReportingDataInput
    {        
        public DateTime? Date { get; set; } // date
        public DateTime DateAndTime { get; set; } // datetime
        public short TagIndex { get; set; } // smallint
        public double Val { get; set; } // float
        public double? PreviousVal { get; set; } // float
        public DateTime? PreviousDateAndTime { get; set; } // datetime
    }
}
