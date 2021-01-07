using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParReports_Indore.Model
{
	public class TagInfo
	{
		public int TagIndex {get;set;}
		public string  TagName { get; set; }
		public int TagType { get; set; } 
		public string Val { get; set; }
		public DateTime DateAndTime { get; set; }
	}
	public class TagDataModel
	{
		public int TagIndex { get; set; }
		public string TagName { get; set; }
		public double Val { get; set; }
		public DateTime DateAndTime { get; set; }
	}
}

