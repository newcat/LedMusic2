using LedMusic2.BrowserInterop;
using LedMusic2.NodeConnection;
using LedMusic2.Nodes;
using LedMusic2.Nodes.NodeModels;
using LedMusic2.Outputs;
using LedMusic2.Reactive;
using LedMusic2.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedMusic2
{
    static class App
    {

        private static BrowserAgent browserAgent;
        private static System.Threading.Timer timer;
        private static object timerLock = new object();

        public static MainViewModel VM { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {

            TypeConverter.Initialize();

            VM = new MainViewModel();
            VM.Initialize();

            browserAgent = new BrowserAgent(VM);
            browserAgent.Connected += clientConnected;
            browserAgent.MessageReceived += messageReceived;
            timer = new System.Threading.Timer(new TimerCallback(tick), null, 0, 1000 / GlobalProperties.Instance.FPS);

        }

        private static void clientConnected(object sender, EventArgs e)
        {
            browserAgent.SendFullState();
        }

        private static void messageReceived(object sender, MessageReceivedEventArgs e)
        {
            var msg = e.Message as JObject;
            switch ((string)msg["type"])
            {
                case "command":
                    VM.HandleCommand((string)msg["command"], msg["payload"]);
                    break;
                case "save":
                    save((string)msg["path"]);
                    break;
                case "load":
                    load((string)msg["path"]);
                    break;
                default:
                    Console.WriteLine("Unknown message type: {0}", msg["type"]);
                    break;
            }
        }

        private static void tick(object state)
        {
            lock (timerLock)
            {
                VM.Tick();
                browserAgent.SendStateUpdates();
            }
        }

        private static void save(string path)
        {
            File.WriteAllText(path, VM.GetFullState().ToJson().ToString());
        }

        private static void load(string path)
        {

            var s = File.ReadAllText(path);
            var jobj = JObject.Parse(s);
            var newVm = new MainViewModel(jobj);
            VM = newVm;
            browserAgent.VM = newVm;
            browserAgent.SendFullState();

        }

    }
}
