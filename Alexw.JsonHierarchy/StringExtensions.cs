using System;

namespace Alexw.JsonHierarchy
{
    public static class StringExtensions
    {
        public static bool Eq(this string original, string compareTo)
        {
            return string.Equals(original, compareTo, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}