using System;
using UnityEngine;

namespace SMultiLangTranslations
{
    internal static class Utils
    {
        internal static MultiLangManager inst => MultiLangManager.Instance;
        internal static Config conf => MultiLangManager.Instance?.Configuration?.Instance;
        internal const StringComparison DefaultStrComparison = StringComparison.InvariantCultureIgnoreCase;
        public static ConsoleColor FromColor(Color32 c)
        {
            int index = c.a > 128 ? 8 : 0; // Bright bit
            index |= (c.r > 64) ? 4 : 0; // Red bit
            index |= (c.g > 64) ? 2 : 0; // Green bit
            index |= (c.b > 64) ? 1 : 0; // Blue bit
            return (ConsoleColor)index;
        }
    }
}
