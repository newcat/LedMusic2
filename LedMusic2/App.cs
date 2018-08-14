using LedMusic2.BrowserInterop;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LedMusic2
{
    class App
    {

        private readonly BrowserAgent browserAgent;
        private readonly Timer timer;

        private App()
        {

            var mvInstance = MainViewModel.Instance;

            var outputNode = new OutputNode();
            var numberNode = new DoubleValueNode();
            var conn = new Connection(numberNode.Outputs[0], outputNode.Inputs[0]);
            mvInstance.Scenes[0].Nodes.Add(outputNode);
            mvInstance.Scenes[0].Nodes.Add(numberNode);
            mvInstance.Scenes[0].Connections.Add(conn);

            browserAgent = new BrowserAgent();
            browserAgent.Connected += clientConnected;
            browserAgent.MessageReceived += messageReceived;
            timer = new Timer(new TimerCallback(tick), null, 0, 1000 / GlobalProperties.Instance.FPS);

        }

        public static void Main(string[] args)
        {
            var app = new App();
            var data = File.ReadAllText("../../test.json");
            var test = Reactive.ReactiveObject.FromJson<MainViewModel>(JObject.Parse(data));
            Console.WriteLine(test.GetFullState().ToJson().ToString());
            Console.ReadLine();
            Console.WriteLine(MainViewModel.Instance.GetFullState().ToJson().ToString());
            Console.ReadLine();
        }

        private void clientConnected(object sender, EventArgs e)
        {
            browserAgent.SendFullState();
        }

        private void messageReceived(object sender, MessageReceivedEventArgs e)
        {
            var msg = e.Message as JObject;
            switch ((string)msg["type"])
            {
                case "command":
                    MainViewModel.Instance.HandleCommand((string)msg["command"], msg["payload"]);
                    break;
                default:
                    Console.WriteLine("Unknown message type: {0}", msg["type"]);
                    break;
            }
        }

        private void tick(object state)
        {
            MainViewModel.Instance.Tick();
            browserAgent.SendStateUpdates();
        }

    }
}
