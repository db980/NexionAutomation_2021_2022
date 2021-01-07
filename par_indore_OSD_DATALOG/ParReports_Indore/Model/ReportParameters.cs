using ParReports_Indore.Logic;
using System;
using System.Collections.Generic;

namespace ParReports_Indore.Model
{

	public class ReportContext
	{
		public ReportContext()
		{
			Criteria = new ReportCriteria();
		}

		public ReportCriteria Criteria { get; set; }

		public object Data { get; set; }

	}

    public class ReportCriteria
    {
        public DateTime ReportDate { get; set; }
        public string ReportType { get; set; }
        public string Action { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Timeframe { get; set; }
        public string Shift { get; set; }
        public LoadInfo LoadInfo { get; set; }

    }

    public class LoadInfo
    {
        public double LoadNumber { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}


