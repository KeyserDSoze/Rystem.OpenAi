﻿namespace System.Text.Json.Serialization
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class JsonPropertyMaximumAttribute : Attribute
    {
        public double Maximum { get; }
        public bool Exclusive { get; }
        public JsonPropertyMaximumAttribute(double maximum, bool exclusive = false)
        {
            Maximum = maximum;
            Exclusive = exclusive;
        }
    }
}
