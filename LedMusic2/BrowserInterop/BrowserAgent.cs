using LedMusic2.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LedMusic2.BrowserInterop
{
    public class BrowserAgent
    {

        public event EventHandler Connected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public MainViewModel VM { get; set; }

        private WebSocket ws;
        private readonly object lockObject = new object();
        private readonly Thread listenerThread;

        public BrowserAgent(MainViewModel vm)
        {
            VM = vm;
            listenerThread = new Thread(new ThreadStart(listenAsync));
            listenerThread.Start();
        }

        public void SendFullState()
        {
            lock (lockObject)
            {
                sendJson(new JObject(
                    new JProperty("type", "fullstate"),
                    new JProperty("state", VM.GetFullState(Guid.NewGuid()).ToJson())
                ));
            }
        }

        public void SendStateUpdates()
        {
            lock (lockObject)
            {
                if (ws == null || ws.State != WebSocketState.Open) return;
                var updates = VM.GetStateUpdates(Guid.NewGuid());
                if (updates == null || updates.Count == 0) return;
                sendJson(new JObject(
                    new JProperty("type", "stateupdate"),
                    new JProperty("state", updates.ToJson())
                ));
            }
        }

        private async void listenAsync()
        {

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:48235/");
            listener.Start();
            Console.WriteLine("[WS] Listening on port 48235");

            while (true)
            {

                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    processRequestAsync(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }

            }
        }

        private async void processRequestAsync(HttpListenerContext listenerContext)
        {

            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            }
            catch (Exception e)
            {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine("[WS] Exception: {0}", e);
                return;
            }

            ws = webSocketContext.WebSocket;
            Connected?.Invoke(this, new EventArgs());
            Console.WriteLine("[WS] Client connected");

            try
            {

                var receiveBuffer = new byte[1024];
                var message = "";
                while (ws.State == WebSocketState.Open)
                {
                    var receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    switch (receiveResult.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            break;
                        case WebSocketMessageType.Text:
                            message += Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                            if (receiveResult.EndOfMessage)
                            {
                                Console.WriteLine("[WS] Message received: {0}", message);
                                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(JObject.Parse(message)));
                                message = "";
                            }
                            break;
                        default:
                            Console.WriteLine("[WS] Unsupported message type {0}", receiveResult.MessageType);
                            break;
                    }
                    Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[WS] Exception: {0}", e);
            }
            finally
            {
                if (ws != null)
                    ws.Dispose();
                Console.WriteLine("[WS] Connection closed");
            }


        }

        private async void sendJson(JToken json)
        {
            var bytes = Encoding.UTF8.GetBytes(json.ToString());
            await ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

    }

    internal static class HelperExtensions
    {
        public static Task GetContextAsync(this HttpListener listener)
        {
            return Task.Factory.FromAsync(listener.BeginGetContext, listener.EndGetContext, TaskCreationOptions.None);
        }
    }

}
