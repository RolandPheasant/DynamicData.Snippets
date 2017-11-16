using System;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringEx
    {
        public static bool Contains(this string source, string toCheck, StringComparison comparison)
        {
            return source.IndexOf(toCheck, comparison) >= 0;
        }
    }
}
