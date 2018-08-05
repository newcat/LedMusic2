using System;

namespace LedMusic2.Reactive
{

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ReactiveIgnoreAttribute : Attribute
    {
    }
}
