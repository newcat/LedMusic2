using System;
using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Sockets;

namespace LedMusicVstPlugin
{
    class LedMusicInterop
    {

        private const int MIDI_PACKET_COUNT = 5;
        private const int MAX_QUEUE_SIZE = 100 * MIDI_PACKET_COUNT;

        private readonly Guid guid;
        private readonly ConcurrentQueue<byte[]> queue = new ConcurrentQueue<byte[]>();
        private MemoryMappedFile mf;
        private MemoryMappedViewAccessor va;
        private readonly UdpClient udpClient = new UdpClient();
        private bool connected = false;
        private DateTime lastSent = DateTime.Now - TimeSpan.FromDays(1);
        private readonly Plugin plugin;

        public LedMusicInterop(Guid guid, Plugin p)
        {
            this.guid = guid;
            plugin = p;
        }

        public void Register()
        {
            if ((DateTime.Now - lastSent).TotalMilliseconds < 5000)
                return;

            var packet = new byte[17];
            Array.Copy(guid.ToByteArray(), packet, 16);
            packet[16] = 1; // Type = MIDI
            udpClient.Send(packet, 17, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1959));
            lastSent = DateTime.Now;
        }

        public void Process(byte[] data)
        {

            if (queue.Count < MAX_QUEUE_SIZE)
            {
                // discard new event if already reached max num of elements
                queue.Enqueue(data);
            }

            if (connected)
            {
                var currentData = va.ReadByte(0);
                if (currentData == 0)
                {
                    for (var i = 0; i < MIDI_PACKET_COUNT; i++)
                    {
                        if (queue.TryDequeue(out byte[] newData))
                            va.WriteArray(3 * i, newData, 0, 3);
                        else
                            break;
                    }
                }
            } else if (udpClient.Available >= 3)
            {
                var ipep = new IPEndPoint(IPAddress.Any, 0);
                var ack = udpClient.Receive(ref ipep);
                udpClient.Dispose();
                mf = MemoryMappedFile.CreateOrOpen($"LedMusicVST_{guid.ToString()}", 3);
                va = mf.CreateViewAccessor();
                connected = true;             
            } else
            {
                Register();
            }

        }

    }
}
