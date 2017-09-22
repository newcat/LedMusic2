using LedMusic2.Models;
using System;

namespace LedMusic2.Outputs
{
    class DummyOutput : OutputBase
    {
        public new static string DefaultName => "Dummy";
   
        public override Type ViewType => null;

        public override void CalculationDone(LedColor[] calculationResult)
        {
            return;
        }
    }
}
