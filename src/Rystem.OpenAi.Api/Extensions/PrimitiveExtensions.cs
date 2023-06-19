using System.Linq;

namespace System.Reflection
{
    public static class PrimitiveExtensions
    {
        private static readonly Type[] s_primitiveTypes = new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
        };
        public static bool IsPrimitive(this Type type)
            => type.IsPrimitive || s_primitiveTypes.Contains(type) || Convert.GetTypeCode(type) != TypeCode.Object ||
            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsPrimitive(type.GetGenericArguments()[0]));
        public static bool IsNumber(this Type type)
            => type == typeof(float) || type == typeof(float?) || type == typeof(double) || type == typeof(double?)
                || type == typeof(decimal) || type == typeof(decimal?);
        public static bool IsInteger(this Type type)
            => type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?)
                || type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?)
                || type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?);
        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
                return false;
            var interfaces = type.GetInterfaces();
            if (type.Name.Contains("IEnumerable`1") || interfaces.Any(x => x.Name.Contains("IEnumerable`1")))
                return true;
            return false;
        }
        public static bool IsDictionary(this Type type)
        {
            var interfaces = type.GetInterfaces();
            if (type.Name.Contains("IDictionary`2") || interfaces.Any(x => x.Name.Contains("IDictionary`2")))
                return true;
            return false;
        }
    }
}
