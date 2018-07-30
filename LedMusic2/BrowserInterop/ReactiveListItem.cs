using System;

namespace LedMusic2.BrowserInterop
{
    public class ReactiveListItem<T> : ReactiveObject, IReactiveListItem
    {
        public Guid Id => throw new NotImplementedException();
        public override string ReactiveName => "ReactiveListItem";
        public ReactivePrimitive<T> Value;
        public ReactiveListItem(T value)
        {
            Value = new ReactivePrimitive<T>(ReactiveName, value);
        }
    }
}
