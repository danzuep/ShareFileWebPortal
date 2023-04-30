using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Common.Helpers
{
    public static class TextExtensions
    {
        // Pluralise with an "s"
        public static string S(this int count) =>
            Math.Abs(count) == 1 ? "" : "s";

        public static string ToPlural(this string word, int count) =>
            Math.Abs(count) == 1 ? word : word.ToPlural();

        public static string ToPlural(this string singular)
        {
            string plural = singular.Clone() as string; //var sb = new StringBuilder(singular);
            // Multiple words in the form A of B : Apply the plural to the first word only (A)
            int index = plural.LastIndexOf(" of ");
            if (index > 0) return (plural.Substring(0, index)) + plural.Remove(0, index).ToPlural();

            // single Word rules
            //sibilant ending rule
            if (plural.EndsWith("sh")) return plural + "es";
            if (plural.EndsWith("ch")) return plural + "es";
            if (plural.EndsWith("us")) return plural + "es";
            if (plural.EndsWith("ss")) return plural + "es";
            //-ies rule
            if (plural.EndsWith("y")) return plural.Remove(plural.Length - 1, 1) + "ies";
            // -oes rule
            if (plural.EndsWith("o")) return plural.Remove(plural.Length - 1, 1) + "oes";
            // -s suffix rule
            return plural + "s";
        }

        public static string ReplaceFirst(this string toSearch, string oldValue, string newValue)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(toSearch))
            {
                int index = toSearch.IndexOf(oldValue);
                result = index < 0 ? toSearch : toSearch.Substring(0, index) +
                    newValue + toSearch.Substring(index + oldValue.Length);
            }
            return result;
        }
    }
}
