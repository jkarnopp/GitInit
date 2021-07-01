using System;

namespace GitInitTest.Common.Extensions
{
    public static class BooleanExtensions
    {
        public static string ToYesNoString(this bool value)
        {
            return value ? Resources.y.ToString() : Resources.n.ToString();
        }

        public static string ToYesNoString(this bool? value)
        {
            return value.HasValue ? value.Value.ToYesNoString() : String.Empty;
        }
    }

    public enum Resources
    {
        n = 0,
        y = 1
    }
}