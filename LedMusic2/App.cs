using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedMusic2
{
    class App
    {

        public static void Main(string[] args)
        {

            var mvm = new MainViewModel();
            var state = mvm.GetFullState();
            state.Print(0);
            Console.ReadLine();

        }

    }
}
