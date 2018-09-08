using LedMusic2.Reactive;
using LedMusic2.LedColors;
using System;

namespace LedMusic2.Outputs
{
    public abstract class OutputBase : ReactiveObject, ICombinedReactive
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public abstract string DefaultName { get; }
        public ReactivePrimitive<string> Name { get; }
            = new ReactivePrimitive<string>();

        protected OutputBase()
        {
            Name.Set(DefaultName);
        }

        public abstract void CalculationDone(LedColorArray calculationResult);

    }
}
