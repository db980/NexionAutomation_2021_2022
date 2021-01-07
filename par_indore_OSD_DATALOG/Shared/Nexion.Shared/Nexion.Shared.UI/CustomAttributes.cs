using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexion.Shared.UI
{
	public class FieldNameAttribute : Attribute
	{
		public FieldNameAttribute() { }

		public FieldNameAttribute(string displayName, int sequence = -1)
		{
			DisplayName = displayName;
			SequenceNumber = sequence;
		}

		public virtual string DisplayName { get; }
		public virtual int SequenceNumber { get; }
	}

}
