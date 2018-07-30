using LedMusic2.LedColors;
using System.Windows;

namespace LedMusic2.Outputs.OutputModels
{
    [Output("Dummy")]
    class DummyOutput : OutputBase
    {
        public override string DefaultName => "Dummy";

        public override void CalculationDone(LedColors.LedColor[] calculationResult)
        {
            return;
        }
    }
}
