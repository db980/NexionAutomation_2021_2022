﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexion.Shared.Data
{
    public static class DataExtensions
    {
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

		public static List<string> ToCSV(this IDataReader dataReader, bool includeHeaderAsFirstRow, string separator)
		{
			List<string> csvRows = new List<string>();
			StringBuilder sb = null;

			if (includeHeaderAsFirstRow)
			{
				sb = new StringBuilder();
				for (int index = 0; index < dataReader.FieldCount; index++)
				{
					if (dataReader.GetName(index) != null)
						sb.Append(dataReader.GetName(index));

					if (index < dataReader.FieldCount - 1)
						sb.Append(separator);
				}
				csvRows.Add(sb.ToString());
			}

			while (dataReader.Read())
			{
				sb = new StringBuilder();
				for (int index = 0; index < dataReader.FieldCount - 1; index++)
				{
					if (!dataReader.IsDBNull(index))
					{
						string value = dataReader.GetValue(index).ToString();
						if (dataReader.GetFieldType(index) == typeof(String))
						{
							//If double quotes are used in value, ensure each are replaced but 2.
							if (value.IndexOf("\"") >= 0)
								value = value.Replace("\"", "\"\"");

							//If separtor are is in value, ensure it is put in double quotes.
							if (value.IndexOf(separator) >= 0)
								value = "\"" + value + "\"";
						}
						sb.Append(value);
					}

					if (index < dataReader.FieldCount - 1)
						sb.Append(separator);
				}

				if (!dataReader.IsDBNull(dataReader.FieldCount - 1))
					sb.Append(dataReader.GetValue(dataReader.FieldCount - 1).ToString().Replace(separator, " "));

				csvRows.Add(sb.ToString());
			}
			dataReader.Close();
			sb = null;
			return csvRows;
		}


	}
}
