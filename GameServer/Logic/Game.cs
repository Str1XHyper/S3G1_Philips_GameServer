using Models.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Logic
{
    public class Game
    {
        private List<Player> players = new List<Player>();
        private Bank bank = new Bank();
        private int currentPlayerIndex = 0;
        private int amountPlayersAnsweredQuestion = 0;
        private List<Question> questions;
        private int currentQuestionIndex = 0;


        public Game(string LessonId)
        {
            if(string.IsNullOrEmpty(LessonId))
            {
                throw new ArgumentException("LessonID cannot be null or empty");
            }
            questions = GetQuestions(LessonId);
        }
        public ResponseObject HandlePlayerJoin(PlayerJoinMessage message, string SessionID)
        {
            bool playerExists = false;
            foreach(Player player in players)
            {
                if(message.playerId == player.PlayerID)
                {
                    playerExists = true;
                    break;
                }
            }

            if (!playerExists)
            {
                Player newPlayer = new Player(message.playerId, SessionID, message.Username);
                players.Add(newPlayer);
            } 
            else
            {
                Player player = GetPlayerFromID(message.playerId);
                player.SetSessionID(SessionID);
            }
            PlayerJoinResponse playerJoinResponse = new PlayerJoinResponse(message.playerId, players);
            ResponseObject responseObject = new ResponseObject()
            {
                ResponseString = JsonSerializer.Serialize(playerJoinResponse),
                sessions = getSessions()
            };
            return responseObject;
        }

        public ResponseObject HandleSocketMessage(string jsonString)
        {
            SocketMessage message = JsonSerializer.Deserialize<SocketMessage>(jsonString);
            string response = string.Empty;
            switch (message.messageType)
            {
                case MessageType.DICE_THROW:
                    response = HandleDiceThrow(JsonSerializer.Deserialize<DiceThrow>(jsonString));
                    break;
                case MessageType.ENCOUNTERED_SPACE:
                    response = HandleSpaceEncounter(JsonSerializer.Deserialize<EncounteredSpace>(jsonString));
                    break;
                case MessageType.TURN_END:
                    response = HandleTurnEnd();
                    break;
                case MessageType.DIRECTION_CHOSEN:
                    response = HandleChooseDirection(JsonSerializer.Deserialize<DirectionChosen>(jsonString));
                    break;
                case MessageType.PASSED_BANK:
                    response = HandlePassBank(JsonSerializer.Deserialize<PassedBank>(jsonString));
                    break;
                case MessageType.PASSED_START:
                    response = HandlePassStart(JsonSerializer.Deserialize<PassedStart>(jsonString));
                    break;
                case MessageType.BOUGHT_STAR:
                    response = HandleStarPurchase(JsonSerializer.Deserialize<BoughtStar>(jsonString));
                    break;
                case MessageType.ANSWERED_QUESTION:
                    response = HandleAnswerQuestion(JsonSerializer.Deserialize<AnsweredQuestion>(jsonString));
                    break;
                case MessageType.START_GAME:
                    response = StartGame();
                    break;
            }
            Console.WriteLine("Sent: " + response);
            ResponseObject responseObject = new ResponseObject()
            {
                ResponseString = response,
                sessions = getSessions()
            };
            return responseObject;
        }

        private string StartGame()
        {
            return JsonSerializer.Serialize(new StartTurnResponse(players[currentPlayerIndex].PlayerID));
        }

        private string HandleAnswerQuestion(AnsweredQuestion answeredQuestion)
        {
            Player player = GetPlayerFromID(answeredQuestion.playerId);
            if (questions[currentQuestionIndex].answers[0].answer == answeredQuestion.answer)
            {
                player.AddPoints(1);
            }
            amountPlayersAnsweredQuestion += 1;
            if(amountPlayersAnsweredQuestion >= players.Count)
            {
                amountPlayersAnsweredQuestion = 0;
                currentQuestionIndex += 1;

                if(currentQuestionIndex >= questions.Count)
                {
                    return JsonSerializer.Serialize(new EndGameResponse(""));
                }

                return JsonSerializer.Serialize(new StartTurnResponse(players[0].PlayerID));
            }
            return "";
        }

        private string HandleTurnEnd()
        {
            currentPlayerIndex += 1;
            if(currentPlayerIndex == players.Count)
            {
                currentPlayerIndex = 0;
                questions[currentQuestionIndex].answers = GetAnswers(questions[currentQuestionIndex].id);
                return JsonSerializer.Serialize(new QuestionResponse("", questions[currentQuestionIndex]));
            } 
            else
            {
                return JsonSerializer.Serialize(new StartTurnResponse(players[currentPlayerIndex].PlayerID));
            }
        }

        private string HandleStarPurchase(BoughtStar boughtStar)
        {
            Player player = GetPlayerFromID(boughtStar.playerId);
            if (CanBuyStar(player))
            {
                player.AddStar(1);
                player.SubtractPoints(5);
            }
            return JsonSerializer.Serialize(CreateScoreResponse());
        }

        private string HandlePassStart(PassedStart passedStart)
        {
            Player player = GetPlayerFromID(passedStart.playerId);
            player.AddPoints(2);
            return JsonSerializer.Serialize(CreateScoreResponse());
        }

        private string HandlePassBank(PassedBank passedBank)
        {
            Player player = GetPlayerFromID(passedBank.playerId);
            int scoreToAddToBank = player.Points > 1 ? 2 : player.Points;
            bank.AddMoneyToBank(scoreToAddToBank);
            player.SubtractPoints(scoreToAddToBank);
            return JsonSerializer.Serialize(CreateScoreResponse());
        }

        private string HandleChooseDirection(DirectionChosen directionChosen)
        {
            return JsonSerializer.Serialize(new DirectionChosenResponse(directionChosen.playerId, directionChosen.direction));
        }

        private string HandleSpaceEncounter(EncounteredSpace encounteredSpace)
        {
            Player player = GetPlayerFromID(encounteredSpace.playerId);
            switch (encounteredSpace.spaceType)
            {
                case SpaceType.BANK:
                    LandOnBank(player);
                    break;
                case SpaceType.GAIN_POINTS:
                    player.AddPoints(1);
                    break;
                case SpaceType.LOSE_POINTS:
                    player.SubtractPoints(1);
                    break;
                case SpaceType.START:
                    player.AddPoints(2);
                    break;
            }
            ScoreResponse scoreResponse = new ScoreResponse(player.PlayerID, player.Points, player.Stars);
            return JsonSerializer.Serialize(scoreResponse);
        }

        private string HandleDiceThrow(DiceThrow diceThrow)
        {
            return JsonSerializer.Serialize(new MovePlayerResponse(diceThrow.playerId, diceThrow.rolledNumber));
        }

        private void LandOnBank(Player player)
        {
            int receivedMoney = bank.LandOnBank();
            player.AddPoints(receivedMoney);
        }

        private List<ScoreResponse> CreateScoreResponse()
        {
            List<ScoreResponse> scoreResponses = new List<ScoreResponse>();
            foreach(Player player in players)
            {
                scoreResponses.Add(new ScoreResponse(player));
            }
            return scoreResponses;
        }

        private bool CanBuyStar(Player player)
        {
            if (player.Points >= 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Player GetPlayerFromID(string playerID)
        {
            foreach (Player player in players)
            {
                if (player.PlayerID == playerID)
                {
                    return player;
                }
            }
            return null;
        }

        private static List<Question> GetQuestions(string LessonID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.str1xhyper.nl/Question/"+LessonID);
            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<Question>>(data);
        }

        private static List<Answer> GetAnswers(string questionID)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.str1xhyper.nl/Answer/" + questionID);
            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = reader.ReadToEnd();
            return JsonSerializer.Deserialize<List<Answer>>(data);
        }

        private List<string> getSessions()
        {
            List<string> response = new List<string>();
            foreach(Player player in players)
            {
                response.Add(player.SessionID);
            }
            return response;
        }
    }
}
