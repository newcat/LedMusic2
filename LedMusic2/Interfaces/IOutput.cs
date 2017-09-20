using LedMusic2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2.Interfaces
{
    interface IOutput
    {

        string Name { get; }
        Type ViewType { get; }

        void CalculationDone(LedColor[] calculationResult);


    }
}
