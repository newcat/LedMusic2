using LedMusic2.Color;
using System.Windows;

namespace LedMusic2.Outputs.OutputModels
{
    [Output("Dummy")]
    class DummyOutput : OutputBase
    {
        public override string DefaultName => "Dummy";

        public override FrameworkElement SettingsView => null;

        public override void CalculationDone(LedColor[] calculationResult)
        {
            return;
        }
    }
}
