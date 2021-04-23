using Logic;
using Models.Message;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameServer
{
    public class Mercier : WebSocketBehavior
    {
        private Game game = new Game();
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
            string response = game.HandleSocketMessage(e.Data);
            Send(response);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            Console.WriteLine("eigen message van de onclose");
        }
    }
    public class Program
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
