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
            message2.Username = "henk";
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


        [TestMethod()]
        public void HandlePlayerJoin_SessionID_Empty()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }

        [TestMethod()]
        public void HandlePlayerJoin_SessionID_Null()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = null;

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }
        [TestMethod()]
        public void HandlePlayerJoin_PlayerID_Empty()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("");
            message.Username = "piet";
            string sessionID = "1";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }

        [TestMethod()]
        public void HandlePlayerJoin_PlayerID_Null()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage(null);
            message.Username = "piet";
            string sessionID = "1";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }

        [TestMethod()]
        public void HandlePlayerJoin_Username_Empty()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "";
            string sessionID = "1";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }

        [TestMethod()]
        public void HandlePlayerJoin_Username_Null()
        {
            //Arrange
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = null;
            string sessionID = "1";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandlePlayerJoin(message, sessionID));
        }


        [TestMethod()]
        public void HandleSocketMessage_DiceThrow_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            DiceThrow diceThrow = new DiceThrow()
            {
                playerId = "1",
                rolledNumber = 1
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new MovePlayerResponse(diceThrow.playerId, diceThrow.rolledNumber));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(diceThrow));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_DiceThrow_PlayerIDEmpty()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            DiceThrow diceThrow = new DiceThrow()
            {
                playerId = "",
                rolledNumber = 1
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(diceThrow)));
        }

        [TestMethod()]
        public void HandleSocketMessage_DiceThrow_PlayerIDNull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            DiceThrow diceThrow = new DiceThrow()
            {
                playerId = null,
                rolledNumber = 1
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(diceThrow)));
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpaceBank_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.BANK
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new ScoreResponse("1", 0, 0));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpaceChooseDirection_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.CHOOSE_DIRECTION
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new ScoreResponse("1", 0, 0));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }
        [TestMethod()]
        public void HandleSocketMessage_EncounterSpaceGainPoints_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.GAIN_POINTS
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new ScoreResponse("1", 1, 0));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpaceLosePoints_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.BANK
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new ScoreResponse("1", 0, 0));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpaceStart_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.START
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion


            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new ScoreResponse("1", 2, 0));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpace_PlayerIDEmpty()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "",
                spaceType = SpaceType.BANK
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace)));
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpace_PlayerIDNull ()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = null,
                spaceType = SpaceType.BANK
            };
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace)));
        }
        [TestMethod()]
        public void HandleSocketMessage_TurnEnd_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_DirectionChosen_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_PassedBank_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_PassedStart_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_BoughtStar_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_AnsweredQuestion_Successfull()
        {

        }
        [TestMethod()]
        public void HandleSocketMessage_StartGame_Successfull()
        {

        }
    }
}