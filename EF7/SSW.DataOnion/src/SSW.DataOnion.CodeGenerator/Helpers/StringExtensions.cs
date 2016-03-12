using SSW.DataOnion.CodeGenerator.Exceptions;

namespace SSW.DataOnion.CodeGenerator.Helpers
{
    public static class StringExtensions
    {
        public static string Pluralize(this string input)
        {
            Guard.AgainstNullOrEmptyString(input, nameof(input));

            if (input.EndsWith("y"))
            {
                input = input.Substring(0, input.Length - 1) + "ies";
            }
            else if (input.EndsWith("s"))
            {
                input = input + "es";
            }
            else
            {
                input = input + "s";
            }

            return input;
        }
    }
}
