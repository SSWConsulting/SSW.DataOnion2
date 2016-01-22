using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSW.DataOnion.EF6
{
    public class DataOperationException : Exception
    {
        public DataOperationException(string message)
          : base(message)
        {
        }

        public DataOperationException(string message, Exception ex)
          : base(message, ex)
        {
        }
    }
}
