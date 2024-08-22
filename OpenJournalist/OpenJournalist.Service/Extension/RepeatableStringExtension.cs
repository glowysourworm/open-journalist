using System;

using Google.Apis.Util;

using WpfCustomUtilities.Extensions;

namespace OpenJournalist.Service.Extension
{
    public static class RepeatableStringExtension
    {
        public static Repeatable<string> ToRepeatable(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return new Repeatable<string>(Array.Empty<string>());


            var pieces = theString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            return new Repeatable<string>(pieces);
        }
        public static Repeatable<string> ToRepeatable(this string theString, params string[] separators)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return new Repeatable<string>(Array.Empty<string>());


            var pieces = theString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return new Repeatable<string>(pieces);
        }

        /// <summary>
        /// Returns repeatable string from any enum type with [Flags] attribute, for the values that have flags set.
        /// </summary>
        /// <exception cref="Exception">Improper use of Enum types</exception>
        public static Repeatable<string> ToRepeatable<T>(this T enumWithFlags) where T : Enum
        {
            // TODO: FIX THIS PROBLEM, flagged enum extensions need to be overhauled
            //if (!enumWithFlags.IsFlagEnum<T>())
            //    throw new Exception("Can only create repeatable string from Enum with [Flags] attribute");

            return new Repeatable<string>(enumWithFlags.GetFlaggedNames<T>());
        }
    }
}
