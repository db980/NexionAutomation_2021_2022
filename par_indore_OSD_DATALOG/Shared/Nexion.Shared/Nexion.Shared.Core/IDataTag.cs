using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexion.Shared.Core
{
    /// <summary>
    /// interface for all classes to support conversion to larger time frame
    /// </summary>
    public interface IDataTag
    {
        DateTime DateAndTime { get; set; }
    }
}
