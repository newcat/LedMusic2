using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace LedMusic2.VstInterop
{
    public class VstInputManager : VMBase
    {

        private const int PORT = 1959;

        #region Singleton
        private static VstInputManager _instance;
        public static VstInputManager Instance {
            get
            {
                if (_instance == null) _instance = new VstInputManager();
                return _instance;
            }
        }
        #endregion

        private Dictionary<Guid, VstChannel> channelDictionary = new Dictionary<Guid, VstChannel>();
        private ObservableCollection<VstChannel> _channels = new ObservableCollection<VstChannel>();
        public ObservableCollection<VstChannel> Channels => _channels;

        private Thread listenerThread;
        private UdpClient udpClient = new UdpClient(PORT);

        private VstInputManager()
        {
            listenerThread = new Thread(new ThreadStart(Listen));
            listenerThread.Start();
        }

        private void Listen()
        {

            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, PORT);

            while (true)
            {

                var datagram = udpClient.Receive(ref ipEndpoint);

                try
                {

                    var message = JObject.Parse(Encoding.UTF8.GetString(datagram));
                    var uuid = message.Value<string>("uuid");
                    var type = message.Value<string>("type");

                    var guid = Guid.Parse(uuid);
                    if (!channelDictionary.TryGetValue(guid, out VstChannel channel))
                    {
                        channel = new VstChannel(guid);
                        channelDictionary.Add(guid, channel);
                        Channels.Add(channel);
                    }

                    channel.ExecuteMessage(message);

                } catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }

            }

        }

    }
}
