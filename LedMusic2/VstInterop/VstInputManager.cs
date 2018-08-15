using LedMusic2.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LedMusic2.Reactive;

namespace LedMusic2.VstInterop
{
    public class VstInputManager : ReactiveObject
    {

        private const int PORT = 1959;

        private Dictionary<Guid, VstChannel> channelDictionary = new Dictionary<Guid, VstChannel>();
        public ReactiveCollection<VstChannel> Channels { get; } = new ReactiveCollection<VstChannel>();

        private Thread listenerThread;
        private UdpClient udpClient;
        private readonly ConcurrentQueue<VstChannel> channelsToAdd = new ConcurrentQueue<VstChannel>();

        public VstInputManager()
        {

            try
            {
                udpClient = new UdpClient(PORT);
                listenerThread = new Thread(new ThreadStart(listen));
                listenerThread.Start();
            } catch (SocketException)
            {
                Debug.WriteLine("Socket exception");
            }

        }

        public void UpdateValues()
        {
            while (channelsToAdd.TryDequeue(out VstChannel c))
                Channels.Add(c);
            foreach (var c in Channels)
                c.UpdateValues();
        }

        public void Shutdown()
        {
            listenerThread.Abort();
            udpClient.Close();
        }

        // "Hello" packet format
        // Byte 0 - 15: GUID
        // Byte 16: Type
        // Expect "ACK" packet:
        // Byte 0 - 3: ASCII-encoded "ACK"
        private void listen()
        {

            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, PORT);

            while (true)
            {

                var datagram = udpClient.Receive(ref ipEndpoint);
                var guid = new Guid(datagram.Take(16).ToArray());
                var type = (VstChannelType)datagram[16];

                // see if we already have a VstChannel instance for the specified GUID
                if (channelDictionary.TryGetValue(guid, out VstChannel channel))
                {
                    // check if types match
                    if (channel.Type.Get() != type)
                    {
                        Debug.WriteLine($"Incompatible types for channel {guid.ToString()}: Expected {channel.Type}, got {type}");
                        continue;
                    }
                } else
                {
                    channel = new VstChannel(guid, type);
                    channelDictionary.Add(guid, channel);
                    channelsToAdd.Enqueue(channel);
                }

                udpClient.Send(Encoding.ASCII.GetBytes("ACK"), 3, ipEndpoint);
                Debug.WriteLine($"Plugin {guid} registered");

            }

        }

    }
}
