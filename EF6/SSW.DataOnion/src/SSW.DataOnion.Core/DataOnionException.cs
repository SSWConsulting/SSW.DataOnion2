using System;

namespace SSW.DataOnion.Core
{
    public class DataOnionException : Exception
    {
        public DataOnionException(string message) : base(message)
        {
        }

        public DataOnionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
