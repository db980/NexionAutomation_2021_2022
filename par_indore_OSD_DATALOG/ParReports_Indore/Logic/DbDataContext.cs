using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParReports_Indore.Logic
{
	public class DBDataContext : DataContext
	{
		private static MappingSource mappingSource = (MappingSource)new AttributeMappingSource();

		//public DBDataContext()
		//  : base("Data Source=DELL-PC\\FTVIEWX64TAGDB;Initial Catalog=NFLDataBase;Integrated Security=True;User ID=sa; Password=Atc@12345678;", DBDataContext.mappingSource)
		//{
		//}

		public DBDataContext(string connection)
		  : base(connection, DBDataContext.mappingSource)
		{
		}

		public DBDataContext(IDbConnection connection)
		  : base(connection, DBDataContext.mappingSource)
		{
		}

		public DBDataContext(string connection, MappingSource mappingSource)
		  : base(connection, mappingSource)
		{
		}

		public DBDataContext(IDbConnection connection, MappingSource mappingSource)
		  : base(connection, mappingSource)
		{
		}

	}
}
