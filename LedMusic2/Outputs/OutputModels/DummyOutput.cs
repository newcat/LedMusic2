using LedMusic2.LedColors;

namespace LedMusic2.Outputs.OutputModels
{
    [Output("Dummy")]
    class DummyOutput : OutputBase
    {
        public override string DefaultName => "Dummy";

        public override void CalculationDone(LedColorArray calculationResult)
        {
            return;
        }
    }
}
