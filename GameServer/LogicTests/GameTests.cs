using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Models.Message;
using System.Text.Json;
using System.Net;
using System.IO;
using Models.Response;

namespace Logic.Tests
{
    [TestClass()]
    public class GameTests
    {
        private static List<Question> GetQuestions(string LessonID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.str1xhyper.nl/Question/" + LessonID);
            WebResponse webResponse = null;
            while (webResponse == null)
            {
                try
                {
                    webResponse = request.GetResponse();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receiving questions failed with exception:" + ex.Message);
                    Console.WriteLine("Retrying...");
                    webResponse = null;
                }
            }
            using var webStream = webResponse.GetResponseStream();
            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<Question>>(data);
        }

        private static List<Answer> GetAnswers(string questionID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.str1xhyper.nl/Answer/" + questionID);

            WebResponse webResponse = null;
            while (webResponse == null)
            {
                try
                {
                    webResponse = request.GetResponse();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receiving answers failed with exception:" + ex.Message);
                    Console.WriteLine("Retrying...");
                    webResponse = null;
                }
            }
            using var webStream = webResponse.GetResponseStream();
            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<Answer>>(data);
        }


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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.BANK
            };

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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.CHOOSE_DIRECTION
            };

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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.GAIN_POINTS
            };


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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.BANK
            };


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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "1",
                spaceType = SpaceType.START
            };

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
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = "",
                spaceType = SpaceType.BANK
            };

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace)));
        }

        [TestMethod()]
        public void HandleSocketMessage_EncounterSpace_PlayerIDNull ()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            EncounteredSpace encounteredSpace = new EncounteredSpace()
            {
                playerId = null,
                spaceType = SpaceType.BANK
            };

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(encounteredSpace)));
        }
        [TestMethod()]
        public void HandleSocketMessage_TurnEndLastPlayer_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            TurnEnd turnEnd = new TurnEnd("1", 0);
            ResponseObject expected = new ResponseObject();
            Question expectedQuestion = GetQuestions("ac04dcab-b025-45ff-b90a-d15b73759284")[0];
            expectedQuestion.answers = GetAnswers(expectedQuestion.id);

            expected.ResponseString = JsonSerializer.Serialize(new QuestionResponse("", expectedQuestion));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_TurnEndNotLastPlayer_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage playerJoinMessage1 = new PlayerJoinMessage("1");
            playerJoinMessage1.Username = "piet";
            string sessionID1 = "15";
            game.HandlePlayerJoin(playerJoinMessage1, sessionID1);

            PlayerJoinMessage playerJoinMessage2 = new PlayerJoinMessage("2");
            playerJoinMessage2.Username = "klaas";
            string sessionID2 = "15";
            game.HandlePlayerJoin(playerJoinMessage2, sessionID2);
            #endregion

            TurnEnd turnEnd = new TurnEnd("1", 0);
            ResponseObject expected = new ResponseObject();

            expected.ResponseString = JsonSerializer.Serialize(new StartTurnResponse("2"));
            expected.sessions = new List<string>() { sessionID1, sessionID2 };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_TurnEnd2PlayerLastPlayer_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage playerJoinMessage1 = new PlayerJoinMessage("1");
            playerJoinMessage1.Username = "piet";
            string sessionID1 = "15";
            game.HandlePlayerJoin(playerJoinMessage1, sessionID1);

            PlayerJoinMessage playerJoinMessage2 = new PlayerJoinMessage("2");
            playerJoinMessage2.Username = "klaas";
            string sessionID2 = "15";
            game.HandlePlayerJoin(playerJoinMessage2, sessionID2);
            #endregion

            TurnEnd turnEnd1 = new TurnEnd("1", 0);
            TurnEnd turnEnd2 = new TurnEnd("2", 0);
            ResponseObject expected = new ResponseObject();
            Question expectedQuestion = GetQuestions("ac04dcab-b025-45ff-b90a-d15b73759284")[0];
            expectedQuestion.answers = GetAnswers(expectedQuestion.id);

            expected.ResponseString = JsonSerializer.Serialize(new QuestionResponse("", expectedQuestion));
            expected.sessions = new List<string>() { sessionID1, sessionID2 };

            //Act
            game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd1));
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd2));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_TurnEnd_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            TurnEnd turnEnd = new TurnEnd("", 0);


            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd)));
        }

        [TestMethod()]
        public void HandleSocketMessage_TurnEnd_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            TurnEnd turnEnd = new TurnEnd(null, 0);


            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd)));
        }

        [TestMethod()]
        public void HandleSocketMessage_TurnEnd_NotCurrentTurnPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            TurnEnd turnEnd = new TurnEnd("2", 0);


            //Assert
            Assert.ThrowsException<AccessViolationException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(turnEnd)));
        }

        [TestMethod()]
        public void HandleSocketMessage_DirectionChosen_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            DirectionChosen directionChosen = new DirectionChosen();
            directionChosen.direction = MovementDirection.DOWN;
            directionChosen.playerId = "1";
            ResponseObject expected = new ResponseObject();

            expected.ResponseString = JsonSerializer.Serialize(new DirectionChosenResponse(directionChosen.playerId, directionChosen.direction));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(directionChosen));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_DirectionChosen_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            DirectionChosen directionChosen = new DirectionChosen();
            directionChosen.direction = MovementDirection.DOWN;
            directionChosen.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(directionChosen)));
        }

        [TestMethod()]
        public void HandleSocketMessage_DirectionChosen_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            DirectionChosen directionChosen = new DirectionChosen();
            directionChosen.direction = MovementDirection.DOWN;
            directionChosen.playerId = null;

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(directionChosen)));
        }


        [TestMethod()]
        public void HandleSocketMessage_PassedBank_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedBank passedBank = new PassedBank();
            passedBank.playerId = "1";

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1", 0, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(scores);
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(passedBank));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);

        }

        [TestMethod()]
        public void HandleSocketMessage_PassedBank_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedBank passedBank = new PassedBank();
            passedBank.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(passedBank)));
        }

        [TestMethod()]
        public void HandleSocketMessage_PassedBank_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedBank passedBank = new PassedBank();
            passedBank.playerId = null;

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(passedBank)));
        }

        [TestMethod()]
        public void HandleSocketMessage_PassedStart_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedStart passedStart = new PassedStart();
            passedStart.playerId = "1";

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1",2, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(scores);
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(passedStart));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_PassedStart_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedStart passedStart = new PassedStart();
            passedStart.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(passedStart)));
        }

        [TestMethod()]
        public void HandleSocketMessage_PassedStart_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedStart passedStart = new PassedStart();
            passedStart.playerId = null;

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(passedStart)));
        }

        [TestMethod()]
        public void HandleSocketMessage_BoughtStar_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            PassedStart passedStart = new PassedStart();
            passedStart.playerId = "1";

            BoughtStar boughtStar = new BoughtStar();
            boughtStar.playerId = "1";

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1",1, 1)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(scores);
            expected.sessions = new List<string>() { sessionID };

            //Act
            game.HandleSocketMessage(JsonSerializer.Serialize(passedStart));
            game.HandleSocketMessage(JsonSerializer.Serialize(passedStart));
            game.HandleSocketMessage(JsonSerializer.Serialize(passedStart));
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(boughtStar));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);

        }

        [TestMethod()]
        public void HandleSocketMessage_BoughtStar_NotEnoughMoney()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            BoughtStar boughtStar = new BoughtStar();
            boughtStar.playerId = "1";

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1",0, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(scores);
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(boughtStar));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_BoughtStar_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            BoughtStar boughtStar = new BoughtStar();
            boughtStar.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(boughtStar)));
        }

        [TestMethod()]
        public void HandleSocketMessage_BoughtStar_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            BoughtStar boughtStar = new BoughtStar();
            boughtStar.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(boughtStar)));
        }

        [TestMethod()]
        public void HandleSocketMessage_AnsweredQuestion_Correct_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            AnsweredQuestion answeredQuestion = new AnsweredQuestion();
            answeredQuestion.answer = "10";
            answeredQuestion.playerId = "1";

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new StartTurnResponse("1"));
            expected.sessions = new List<string>() { sessionID };

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1", 1, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");


            ResponseObject expectedScore = new ResponseObject();
            expectedScore.ResponseString = JsonSerializer.Serialize(scores);
            expectedScore.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(answeredQuestion));
            var scoreResponse = game.HandleSocketMessage(JsonSerializer.Serialize(new SocketMessage() { messageType = MessageType.GET_SCORE }));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
            Assert.AreEqual(expectedScore.ResponseString, scoreResponse.ResponseString);
            CollectionAssert.AreEqual(expectedScore.sessions, scoreResponse.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_AnsweredQuestion_Incorrect_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            AnsweredQuestion answeredQuestion = new AnsweredQuestion();
            answeredQuestion.answer = "12";
            answeredQuestion.playerId = "1";

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new StartTurnResponse("1"));
            expected.sessions = new List<string>() { sessionID };

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1", 0, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");


            ResponseObject expectedScore = new ResponseObject();
            expectedScore.ResponseString = JsonSerializer.Serialize(scores);
            expectedScore.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(answeredQuestion));
            var scoreResponse = game.HandleSocketMessage(JsonSerializer.Serialize(new SocketMessage() { messageType = MessageType.GET_SCORE }));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
            Assert.AreEqual(expectedScore.ResponseString, scoreResponse.ResponseString);
            CollectionAssert.AreEqual(expectedScore.sessions, scoreResponse.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_AnsweredQuestion_EmptyPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            AnsweredQuestion answeredQuestion = new AnsweredQuestion();
            answeredQuestion.answer = "12";
            answeredQuestion.playerId = "";

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(answeredQuestion)));
        }

        [TestMethod()]
        public void HandleSocketMessage_AnsweredQuestion_NullPlayerID()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            AnsweredQuestion answeredQuestion = new AnsweredQuestion();
            answeredQuestion.answer = "12";
            answeredQuestion.playerId = null;

            //Assert
            Assert.ThrowsException<ArgumentException>(() => game.HandleSocketMessage(JsonSerializer.Serialize(answeredQuestion)));
        }


        [TestMethod()]
        public void HandleSocketMessage_StartGame_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            StartGame startGame = new StartGame();

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(new StartTurnResponse("1"));
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(startGame));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }

        [TestMethod()]
        public void HandleSocketMessage_GetScore_Successfull()
        {
            //Arrange

            #region GameSetup
            Game game = new Game("ac04dcab-b025-45ff-b90a-d15b73759284");
            PlayerJoinMessage message = new PlayerJoinMessage("1");
            message.Username = "piet";
            string sessionID = "15";
            game.HandlePlayerJoin(message, sessionID);
            #endregion

            SocketMessage getScore = new SocketMessage();
            getScore.messageType = MessageType.GET_SCORE;

            List<ScoreResponse> scoreResponses = new List<ScoreResponse>()
            {
                new ScoreResponse("1", 0, 0)
            };
            Scores scores = new Scores(scoreResponses.ToArray(), "");

            ResponseObject expected = new ResponseObject();
            expected.ResponseString = JsonSerializer.Serialize(scores);
            expected.sessions = new List<string>() { sessionID };

            //Act
            var response = game.HandleSocketMessage(JsonSerializer.Serialize(getScore));

            //Assert
            Assert.AreEqual(expected.ResponseString, response.ResponseString);
            CollectionAssert.AreEqual(expected.sessions, response.sessions);
        }
    }
}