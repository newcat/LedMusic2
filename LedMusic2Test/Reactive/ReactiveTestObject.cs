using LedMusic2.Reactive;

namespace LedMusic2Test.Reactive
{
    class ReactiveTestObject : ReactiveObject
    {

        public ReactivePrimitive<int> TestValue { get; } = new ReactivePrimitive<int>(5);

    }
}
