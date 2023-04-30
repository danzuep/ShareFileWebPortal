using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class ExtensionMethods
    {
        public static bool EqualsThis(this string strA, string strB) =>
            string.Equals(strA, strB, StringComparison.OrdinalIgnoreCase);

        public static bool IsValid<T>(T person) => person != null; //[NotNullWhen(true)]

        public static TOut ChangeType<TIn, TOut>(this TIn value) where TOut : class, new()
            => (TOut)Convert.ChangeType(value, typeof(TOut)) ?? new TOut();

        //public static T ShallowCopy<T>(this object o) where T : class => (T)o.MemberwiseClone();
    }
}
