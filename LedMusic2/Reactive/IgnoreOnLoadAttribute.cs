using System;

namespace LedMusic2.Reactive
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreOnLoadAttribute : Attribute
    {
    }
}
