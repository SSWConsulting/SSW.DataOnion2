using System;

namespace SSW.DataOnion.CodeGenerator.Exceptions
{
    public class Guard
    {
        public static void AgainstNullOrEmptyString(string value, string paramName)
        {
            if (!string.IsNullOrEmpty(value)) return;

            var exception = new GenerationException($"'{paramName}' cannot be null or empty",
                new ArgumentNullException(paramName));
            throw exception;
        }
    }
}
