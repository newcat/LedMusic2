using LedMusic2.LedColors;
using LedMusic2.Reactive;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace LedMusic2.Outputs.OutputModels
{

    [Output("TCP")]
    class TcpOutput : OutputBase
    {

        public override string DefaultName => "TCP";

        public ReactivePrimitive<int> Port { get; } = new ReactivePrimitive<int>(4444);

        public TcpOutput() { }
        public TcpOutput(JToken j)
        {
            LoadState(j);
        }

        public override void CalculationDone(LedColorArray calculationResult)
        {
            //TODO
            return;
        }

    }
}
