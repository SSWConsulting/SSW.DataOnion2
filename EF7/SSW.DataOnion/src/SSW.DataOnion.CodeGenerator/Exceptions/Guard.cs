using System;

namespace SSW.DataOnion.CodeGenerator.Exceptions
{
    public class Guard
    {
        public static void AgainstNull<T>(T value, string paramName)
        {
            if (value != null) return;

            var exception = new GenerationException($"'{paramName}' cannot be null",
                new ArgumentNullException(paramName));
            throw exception;
        }

        public static void AgainstNullOrEmptyString(string value, string paramName)
        {
            if (!string.IsNullOrEmpty(value)) return;

            var exception = new GenerationException($"'{paramName}' cannot be null or empty",
                new ArgumentNullException(paramName));
            throw exception;
        }

        public static void AgainstNullOrEmptyGuid(Guid value, string paramName)
        {
            if (value != Guid.Empty) return;

            var exception = new GenerationException($"'{paramName}' cannot be null or empty",
                new ArgumentNullException(paramName));
            throw exception;
        }
    }
}
