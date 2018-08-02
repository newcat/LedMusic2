using System;

namespace LedMusic2.Reactive
{

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ReactiveIgnoreAttribute : Attribute
    {
    }
}
