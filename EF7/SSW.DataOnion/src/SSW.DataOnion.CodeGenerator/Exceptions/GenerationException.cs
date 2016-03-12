using System;

namespace SSW.DataOnion.CodeGenerator.Exceptions
{
    public class GenerationException : Exception
    {
        public GenerationException(string message) : base(message)
        {
        }

        public GenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
