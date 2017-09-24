using LedMusic2.Attributes;
using LedMusic2.Models;
using System.Windows;

namespace LedMusic2.Outputs
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
