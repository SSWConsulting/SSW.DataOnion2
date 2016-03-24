using System;
using System.Collections.Generic;
using System.Linq;

namespace SSW.DataOnion.Core
{
    public class Guard
    {
        public static void AgainstNullOrEmptyString(string value, string paramName)
        {
            if (!string.IsNullOrEmpty(value)) return;

            var exception = new DataOnionException($"'{paramName}' cannot be null or empty",
                new ArgumentNullException(paramName));
            throw exception;
        }

        public static void AgainstNullOrEmptyGuid(Guid value, string paramName)
        {
            if (value != Guid.Empty) return;

            var exception = new DataOnionException($"'{paramName}' cannot be null or empty",
                new ArgumentNullException(paramName));
            throw exception;
        }

        public static void Against(Func<IEnumerable<string>> failureReasonsFunc, string message)
        {
            var reasons = failureReasonsFunc().ToArray();
            if (!reasons.Any()) { return; }

            throw new DataOnionException(message);
        }

        public static void AgainstNull<T>(T value, string paramName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void AgainstNull<T>(T value, string paramName, string message) where T : class
        {
            if (value == null)
            {
                throw new DataOnionException(message, new ArgumentNullException(paramName));
            }
        }
    }
}
