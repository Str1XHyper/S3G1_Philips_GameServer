using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameServer
{
    public class Mercier : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "Test"
                      ? "I've been balused already..."
                      : "I'm not available now.";

            Send(msg);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var wssv = new WebSocketServer("ws://localhost:4000");
            wssv.AddWebSocketService<Mercier>("/Mercier");
            wssv.Start();
            Console.ReadKey(true);
            wssv.Stop();
        }
    }
}
