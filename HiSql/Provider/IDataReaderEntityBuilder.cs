using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    public class IDataReaderEntityBuilder<T>
    {
        private List<string> ReaderKeys { get; set; }

        private IDataReaderEntityBuilder<T> DynamicBuilder;
    }
}
