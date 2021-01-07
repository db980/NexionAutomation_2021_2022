using ParReports_Indore.Model;
using Nexion.Shared.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ParReports_Indore
{
    public static class Util
    {
        public static AppProfile AppProfile { get; set; }

        public static void Serialize<T>(T obj, string filepath)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            TextWriter writer = new StreamWriter(filepath);
            xmlSerializer.Serialize(writer, obj);
            writer.Close();
        }

        public static string[][] ToTwoDimArray(this string source, char separatorOuter = ';', char separatorInner = ',')
        {
            return source
                   .Split(separatorOuter)
                   .Select(x => x.Split(separatorInner))
                   .ToArray();
        }

        public static T Deserialize<T>(string filepath)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            T obj = default(T);
            using (Stream reader = new FileStream(filepath, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                obj = (T)xmlSerializer.Deserialize(reader);
            }
            return obj;
        }

        public static int ToInt32(this IDataReader dr, string columnname)
        {
            return Convert.ToInt32(dr[columnname]);
        }

        public static string ToStringValue(this IDataReader dr, string columnname)
        {
            return Convert.ToString(dr[columnname]);
        }

        public static double ToDoubleValue(this string val)
        {
            return string.IsNullOrWhiteSpace(val) ? 0d : Convert.ToDouble(val);
        }

        public static DateTime ToDateTime(this IDataReader dr, string columnname)
        {
            var data = dr[columnname];

            return data == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr[columnname]);
        }

        public static string CastValues(this Dictionary<int, TagInfo> map, int fieldId)
        {
            return map.ContainsKey(fieldId) ? map[fieldId].Val : string.Empty;
        }
        public static string CastYesNoValues(this Dictionary<int, TagInfo> map, int fieldId)
        {
            return map.ContainsKey(fieldId) ? map[fieldId].Val == "1" ? "OK" : "NOK" : string.Empty;
        }

        public static string CastOnOffValues(this string val)
        {
            return val == "1" ? "ON" : "OFF";
        }

        public static string CastAutoManualValues(this string val)
        {
            return val == "1" ? "Auto" : "Manual";
        }

        public static string CastVFDAutoManualValues(this string val)
        {
            return val == "1" ? "Manual" : "Auto";
        }

        public static string CastValues(this IEnumerable<TagInfo> map, int fieldId)
        {
            return map.FirstOrDefault(a => a.TagIndex == fieldId).Val;

        }

        public static string CastYesNoValues(this IEnumerable<TagInfo> map, int fieldYesId, int fieldNoId = -1)
        {
            string val = map.FirstOrDefault(a => a.TagIndex == fieldYesId).Val;

            if (fieldNoId == -1)
                return val == "1" ? "OK" : "NOK";

            if (val == "1")
                return "OK";
            else
            {
                val = map.FirstOrDefault(a => a.TagIndex == fieldNoId).Val;
                if (val == "1")
                    return "NOK";
            }

            return "OK";
        }

        public static Shift GetShiftTimings(this Shift row)
        {
            //if (row.ShiftName == Shifts.ShiftA)
            //{
            //    return new Shift
            //    {
            //        ShiftName = row.ShiftName,
            //        StartDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.StartTime}"),
            //        EndDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.EndTime}")
            //    };
            //}

            //if (row.ShiftName == Shifts.ShiftB)
            //{
            //    return new Shift
            //    {
            //        ShiftName = row.ShiftName,
            //        StartDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.StartTime}"),
            //        EndDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.EndTime}")
            //    };

            //}
            //if (row.ShiftName == Shifts.ShiftC)
            //{
            //    return new Shift
            //    {
            //        ShiftName = row.ShiftName,
            //        StartDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.StartTime}"),
            //        EndDateTime = Convert.ToDateTime($"{DateTime.Now.AddDays(1).ToString("dd-MMM-yy")} {row.EndTime}")
            //    };
            //}
            //if (row.ShiftName == Shifts.ShiftAll)
            //{
            //    return new Shift
            //    {
            //        ShiftName = row.ShiftName,
            //        StartDateTime = Convert.ToDateTime($"{DateTime.Now.ToString("dd-MMM-yy")} {row.StartTime}"),
            //        EndDateTime = Convert.ToDateTime($"{DateTime.Now.AddDays(1).ToString("dd-MMM-yy")} {row.EndTime}")
            //    };
            //}
            return null;
        }

    }

    public partial class Shift
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public partial class MappingAHULog 
    {
        public Dictionary<string, string> ValuesToReplace { get; set; }
    }


    public partial class TrendMappingDEHLog
    {
        public Dictionary<string, string> ValuesToReplace { get; set; }
    }


    public static partial class DataReportDBStoredProcedures
    {
        public partial class GetAHULogDataResult : ICloneable, IDataTag
        {
            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }



    }

    public static class ReportTypes
    {
        //public const string AHU101 = "MT0022 AHU-101";
        //public const string AHU102 = "MT0023 AHU-102";
        //public const string AHU103 = "MT0024 AHU-103";
        //public const string AHU105 = "MT0026 AHU-105";
        //public const string AHU106 = "MT0027 AHU-106";
        //public const string AHU107 = "MT0028 AHU-107";
        //public const string AHU110 = "MT0031 AHU-110";
        //public const string AHU111 = "MT0032 AHU-111";
        //public const string AHU112 = "MT0033 AHU-112";
        //public const string AHU125 = "MT0046 AHU-125";
        //public const string AHU130 = "MT0051 AHU-130";







public const string MT0022 = "MT0022 AHU-101";
public const string MT0023 = "MT0023 AHU-102";
public const string MT0024 = "MT0024 AHU-103";
public const string MT0025 = "MT0025 AHU-104";
public const string MT0026 = "MT0026 AHU-105";
public const string MT0027 = "MT0027 AHU-106";
public const string MT0028 = "MT0028 AHU-107";
public const string MT0029 = "MT0029 AHU-108";
public const string MT0030 = "MT0030 AHU-109";
public const string MT0031 = "MT0031 AHU-110";
public const string MT0032 = "MT0032 AHU-111";
public const string MT0033 = "MT0033 AHU-112";
public const string MT0034 = "MT0034 AHU-113";
public const string MT0035 = "MT0035 AHU-114";
public const string MT0036 = "MT0036 AHU-115";
public const string MT0037 = "MT0037 AHU-116";
public const string MT0038 = "MT0038 AHU-117";
public const string MT0039 = "MT0039 AHU-118";
public const string MT0040 = "MT0040 AHU-119";
public const string MT0041 = "MT0041 AHU-120";
public const string MT0042 = "MT0042 AHU-121";
public const string MT0043 = "MT0043 AHU-122";
public const string MT0044 = "MT0044 AHU-123";
public const string MT0045 = "MT0045 AHU-124";
public const string MT0046 = "MT0046 AHU-125";
public const string MT0047 = "MT0047 AHU-126";
public const string MT0048 = "MT0048 AHU-127";
public const string MT0049 = "MT0049 AHU-128";
public const string MT0050 = "MT0050 AHU-129";
public const string MT0051 = "MT0051 AHU-130";



        //public const string MT0025 = "MT0025";
        //public const string MT0029 = "MT0029";
        //public const string MT0030 = "MT0030";
        //public const string MT0034 = "MT0034";
        //public const string MT0035 = "MT0035";
        //public const string MT0036 = "MT0036";
        //public const string MT0037 = "MT0037";
        //public const string MT0038 = "MT0038";
        //public const string MT0039 = "MT0039";
        //public const string MT0040 = "MT0040";
        //public const string MT0041 = "MT0041";
        //public const string MT0042 = "MT0042";
        //public const string MT0043 = "MT0043";
        //public const string MT0044 = "MT0044";

        //public const string MT0045 = "MT0045";
        //public const string MT0047 = "MT0047";
        //public const string MT0048 = "MT0048";
        //public const string MT0049 = "MT0049";
        //public const string MT0050 = "MT0050";







    }

    public static class Shifts
    {
        public const string ShiftA = "A Shift";
        public const string ShiftB = "B Shift";
        public const string ShiftC = "C Shift";
        public const string ShiftAll = "ALL Shift";
    }

    public static class MenuList
    {
        public const string AuditTrail = "Audit &Trail";
        public const string DataReport = "&Data Report";
        public const string ALL = "All";
    }

    public static class Timeframe
    {
        public const string Select = "--Select--";
        public const string OneMin = "1 Min";
        public const string FiveMins = "5 Mins";
        public const string TenMins = "10 Mins";
        public const string FifteenMins = "15 Mins";
        public const string ThirtyMins = "30 Mins";
        public const string OneHour = "1 Hour";
    }



    [Serializable]
    public class GridColumn
    {
        public GridColumn() { }

        public GridColumn(string name, bool selected, int order)
        {
            Name = name;
            Selected = selected;
            Order = order;
        }

        public int Order { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public int ColumnWidth { get; set; }

        public bool Selected { get; set; }
    }

    [Serializable]
    public class AppProfile
    {
        public static string FileName = "Profile.xml";

        public AppProfile()
        {
            AhuColumns = new List<GridColumn>();
            DEHColumns = new List<GridColumn>();

        }

        public void Initialize()
        {
            var properties = typeof(ReportViewModel).GetProperties();

            int cntr = 0;
            //foreach (var headerText in list)
            foreach (var prop in properties.Where(a => a.GetCustomAttribute(typeof(DisplayNameAttribute)) != null))
            {
                var displayName = prop.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                if (displayName == null)
                    continue;

                AhuColumns.Add(new GridColumn(displayName.DisplayName, true, cntr));
            }
        }

        public string Name { get; set; }

        public List<GridColumn> AhuColumns { get; set; }
        public List<GridColumn> DEHColumns { get; set; }

        public string[] MenuList { get; set; }



        public partial class TrendMappingDEHLog
        {
            public Dictionary<string, string> ValuesToReplace { get; set; }
        }
    }
}
