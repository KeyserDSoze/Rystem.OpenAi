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
        public static bool IsPrimitive<T>(this T entity)
            => entity?.GetType().IsPrimitive() ?? typeof(T).IsPrimitive();
        public static bool IsPrimitive(this Type type)
            => type.IsPrimitive || s_primitiveTypes.Contains(type) || type.IsEnum || Convert.GetTypeCode(type) != TypeCode.Object ||
            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsPrimitive(type.GetGenericArguments()[0]));
        public static bool IsNumeric<T>(this T entity)
            => entity?.GetType().IsNumeric() ?? typeof(T).IsNumeric();
        public static bool IsNumeric(this Type type)
            => type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?)
                || type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?)
                || type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?)
                || type == typeof(float) || type == typeof(float?) || type == typeof(double) || type == typeof(double?)
                || type == typeof(decimal) || type == typeof(decimal?);
        public static bool IsArray(this Type type)
        {
            if (type == typeof(string))
                return false;
            if (type.IsArray)
                return true;
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
