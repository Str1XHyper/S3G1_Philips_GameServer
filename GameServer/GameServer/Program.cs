using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameServer
{
    public class Mercier : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Console.WriteLine("Open socket");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);

            Console.WriteLine("eigen message van de OnError");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Exception);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data == "Test"
                      ? "I've been balused already..."
                      : "I'm not available now.";

            Console.WriteLine("eigen message van de OnMessage");

            Send(msg);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            Console.WriteLine("eigen message van de onclose");
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
