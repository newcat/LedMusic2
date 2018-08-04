using LedMusic2.BrowserInterop;
using LedMusic2.Nodes;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            browserAgent = new BrowserAgent();
            browserAgent.Connected += clientConnected;
            browserAgent.MessageReceived += messageReceived;
            timer = new Timer(new TimerCallback(tick), null, 0, 1000 / GlobalProperties.Instance.FPS);
        }

        public static void Main(string[] args)
        {
            new App();
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
