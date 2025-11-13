namespace System.Reflection
{
    public static class PrimitiveExtensions
    {
        public static bool IsNumber(this Type type)
            => type == typeof(float) || type == typeof(float?) || type == typeof(double) || type == typeof(double?)
                || type == typeof(decimal) || type == typeof(decimal?);
        public static bool IsInteger(this Type type)
            => type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?)
                || type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?)
                || type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?);
    }
}
