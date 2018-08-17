using LedMusic2.LedColors;
using Newtonsoft.Json.Linq;

namespace LedMusic2.Outputs.OutputModels
{
    [Output("Dummy")]
    class DummyOutput : OutputBase
    {
        public override string DefaultName => "Dummy";

        public DummyOutput() { }
        public DummyOutput(JToken j)
        {
            LoadState(j);
        }

        public override void CalculationDone(LedColorArray calculationResult)
        {
            return;
        }
    }
}
