using ParReports_Indore.Model;
using Nexion.Shared.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ParReports_Indore.DataReportDBStoredProcedures;

namespace ParReports_Indore.Logic
{

    public class DataClient
    {
        private readonly DataReportDB _db;
        //  public static IDictionary<string, FieldsToExtract> TagsMapping { get; set; }

        public DataClient()
        {
            //string connstr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            PrepareMapping();
            _db = new DataReportDB();
            _db.CommandTimeout = 1000;
        }

        public List<TagInfo> GetTags()
        {
            string connstr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            List<TagInfo> tagsInfo = new List<TagInfo>();
            using (var conn = new System.Data.SqlClient.SqlConnection(connstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT tt.TagName,tt.TagIndex FROM dbo.TagTable tt", conn);
                cmd.CommandTimeout = 500;
                var dr = cmd.ExecuteReader(CommandBehavior.Default);
                while (dr.Read())
                {
                    tagsInfo.Add(new TagInfo
                    {
                        TagIndex = dr.ToInt32("TagIndex"),
                        TagName = dr.ToStringValue("TagName")
                    });
                }
            }
            return tagsInfo;
        }

        public List<TagInfo> GetTagData(DateTime fromTime, DateTime endTime, TagInfo tag)
        {
            List<TagInfo> tagsInfo = new List<TagInfo>();
            string connstr = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connstr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.GetTagInfo", conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 500
                };
                cmd.Parameters.Add(new SqlParameter("@StartTime", fromTime));
                cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));
                cmd.Parameters.Add(new SqlParameter("@TagIndex", tag.TagIndex));
                var dr = cmd.ExecuteReader(CommandBehavior.Default);

                while (dr.Read())
                {
                    tagsInfo.Add(new TagInfo
                    {
                        TagIndex = dr.ToInt32("TagIndex"),
                        TagName = dr.ToStringValue("TagName"),
                        Val = dr.ToStringValue("Val"),
                        DateAndTime = dr.ToDateTime("UpdatedDateTime")
                    });
                }
            }
            return tagsInfo;
        }

        //Vehicle Report WithOut PY Starts
        private static void PrepareMapping()
        {
            //  TagsMapping = new Dictionary<string, FieldsToExtract>();
        }


        public List<GetLastAHULogDataResult> GetLastHWGLogDataLoadWiseResult(DateTime starttime, DateTime endDateTime, string AHUName)
        {
            return _db.GetLastAHULogData(starttime.AddDays(-7), starttime.AddMilliseconds(-1), AHUName).ToList();

        }

        public Task<Dictionary<DateTime, Dictionary<string, string>>> asyncGetLoadWiseHWGLogViewData(DateTime starttime, DateTime endDateTime, string AHUName, int interval, Action<string> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                return getLoadWiseHWGLogViewData(starttime, endDateTime, AHUName, interval, progressHandler);

            }, cancellationToken);
        }

        private Dictionary<DateTime, Dictionary<string, string>> getLoadWiseHWGLogViewData(DateTime starttime, DateTime endDateTime, string AHUName, int interval, Action<string> progressHandler)
        {
            List<GetAHULogDataResult> data = _db.GetAHULogData(starttime, endDateTime, AHUName).ToList();

            ILookup<string, GetAHULogDataResult> lookedup = data.ToLookup(a => a.TagName);


            List<GetAHULogDataResult> aggregatedata = new List<GetAHULogDataResult>();

            foreach (IGrouping<string, GetAHULogDataResult> tagdata in lookedup)
            {
                aggregatedata.AddRange(tagdata.ToList().ConvertToLargerTimeFrame(TimeSpan.FromMinutes(interval)));
            }

            var sorteddata = aggregatedata.ToList().OrderBy(a => a.DateAndTime);

            Dictionary<DateTime, Dictionary<string, string>> dictionary =
                    sorteddata.Pivot(a => a.DateAndTime, a => a.TagName, lst => lst.Max(a => a.Val).ToString());

            return dictionary;
        }

    }


}
