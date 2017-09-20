using LedMusic2.Interfaces;
using LedMusic2.Models;
using System;

namespace LedMusic2.Outputs
{
    class TcpOutput : IOutput
    {
        public string Name => "TCP";

        public Type ViewType => typeof(TcpOutputView);

        public void CalculationDone(LedColor[] calculationResult)
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
