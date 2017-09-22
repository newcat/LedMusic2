﻿using LedMusic2.Models;
using System;

namespace LedMusic2.Outputs
{

    class TcpOutput : OutputBase
    {
        public new static string DefaultName => "TCP";

        public override Type ViewType => typeof(TcpOutputView);

        public override void CalculationDone(LedColor[] calculationResult)
        {
            //TODO
            return;
        }

    }
}
