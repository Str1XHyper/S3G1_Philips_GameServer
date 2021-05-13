using Logic;
using Models.Message;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameServer
{
    public class Mercier : WebSocketBehavior, IWebSocketSession
    {
        Game game = null;
        //private Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
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
            ResponseObject response;
            Console.WriteLine(e.Data);
            SocketMessage message = JsonSerializer.Deserialize<SocketMessage>(e.Data);
            if(message.messageType == MessageType.PLAYER_JOIN)
            {
                PlayerJoinMessage playerJoinMessage = JsonSerializer.Deserialize<PlayerJoinMessage>(e.Data);
                

                if(!Program.GameDict.ContainsKey(playerJoinMessage.LessonId))
                {
                    game = new Game(playerJoinMessage.LessonId);
                    Program.GameDict.Add(playerJoinMessage.LessonId,game);
                } 
                else
                {
                    game = Program.GameDict[playerJoinMessage.LessonId];
                }
                response = game.HandlePlayerJoin(playerJoinMessage, ID);
            }
            else
            {
                response = game.HandleSocketMessage(e.Data);
            }
            foreach(string id in response.sessions)
            {
                Sessions.SendTo(response.ResponseString, id);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            Console.WriteLine("eigen message van de onclose");
        }
    }
    public class Program
    {
        public static Dictionary<string, Game> GameDict = new Dictionary<string, Game>();

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
