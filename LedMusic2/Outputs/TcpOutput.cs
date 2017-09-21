using LedMusic2.Attributes;
using LedMusic2.Models;
using System;

namespace LedMusic2.Outputs
{
    [Output("TCP")]
    class TcpOutput : OutputBase
    {
        public override string DefaultName => "TCP";

        public override Type ViewType => typeof(TcpOutputView);

        public override void CalculationDone(LedColor[] calculationResult)
        {
            //TODO
            return;
        }

    }
}
