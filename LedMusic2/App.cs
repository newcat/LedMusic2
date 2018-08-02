using LedMusic2.Nodes;
using LedMusic2.Nodes.NodeModels;
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
            var json = mvm.GetFullState().ToJson();
            Console.WriteLine(json.ToString());
            //mvm.StopProcessing();
            //mvm.Scenes[0].Nodes.Add(new BooleanNode());
            //mvm.Scenes[0].Nodes.Remove(mvm.Scenes[0].Nodes[0]);
            //mvm.GetStateUpdates().Print(0);
            Console.ReadLine();

        }

    }
}
