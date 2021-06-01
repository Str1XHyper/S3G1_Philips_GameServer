using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Models.Message;
using System.Text.Json;

namespace Logic.Tests
{
    [TestClass()]
    public class GameTests
    {
        [TestMethod()]
        public void CreateGame_Successfull()
        {
            //Act
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");

            //Assert
            Assert.IsNotNull(game);
        }

        [TestMethod()]
        public void CreateGame_EmptyLessonId()
        {
            //Assert
            Assert.ThrowsException<ArgumentException>(() => new Game(""));
        }

        [TestMethod()]
        public void CreateGame_NullLessonId()
        {
            //Assert
            Assert.ThrowsException<ArgumentException>(() => new Game(null));
        }

        [TestMethod()]
        public void HandlePlayerJoin_SinglePlayer_uccessfull()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new PlayerJoinResponse("1", new List<Player>() { new Player(message.playerId, sessionID, message.Username) }));
            expected.sessions = new List<string>() { sessionID };

            //Act
            ResponseObject response = game.HandlePlayerJoin(message, sessionID);

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandlePlayerJoin_Multiplayer_Successfull()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");

            #region Player 1
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            #endregion

            #region Player 2
            PlayerJoinMessage message2 = new PlayerJoinMessage("2");
            message.Username = "henk";
            string sessionID2 = "16";
            #endregion

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new PlayerJoinResponse("2", new List<Player>() { new Player(message.playerId, sessionID, message.Username), new Player(message2.playerId, sessionID2, message2.Username) }));
            expected.sessions = new List<string>() { sessionID, sessionID2 };

            //Act
            game.HandlePlayerJoin(message, sessionID);
            ResponseObject response = game.HandlePlayerJoin(message2, sessionID2);

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }
    }
}