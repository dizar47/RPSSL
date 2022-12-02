using System.Collections.Concurrent;
using System.Net.Http.Json;
using RPSSL.BL.Exceptions;
using RPSSL.BL.Services.Models;
using RPSSL.Common.Entities;
using RPSSL.Common.Enums;

namespace RPSSL.BL.Services
{
    public class GameService
    {
        public delegate void PlayerWonEventHandler(string? playerName);
        public event PlayerWonEventHandler? PlayerWon;

        private const string RANDOM_GENERATOR_SERVICE_URI = "https://codechallenge.boohma.com/random";

        private Lazy<Choice[]> choices = new Lazy<Choice[]>(() => GetChoicesInitialValues());
        private Lazy<HttpClient> httpClient = new Lazy<HttpClient>();
        private Lazy<ConcurrentDictionary<string, int>> leaderBoard = new Lazy<ConcurrentDictionary<string, int>>();

        private enum RoundOutcome
        {
            Draw,
            Player1Win,
            Player2Win
        }

        private static RoundOutcome[,] gameLogic = {
            { RoundOutcome.Draw, RoundOutcome.Player2Win, RoundOutcome.Player1Win, RoundOutcome.Player1Win, RoundOutcome.Player2Win },
            { RoundOutcome.Player1Win, RoundOutcome.Draw, RoundOutcome.Player2Win, RoundOutcome.Player2Win, RoundOutcome.Player1Win },
            { RoundOutcome.Player2Win, RoundOutcome.Player1Win, RoundOutcome.Draw, RoundOutcome.Player1Win, RoundOutcome.Player2Win },
            { RoundOutcome.Player2Win, RoundOutcome.Player1Win, RoundOutcome.Player2Win, RoundOutcome.Draw, RoundOutcome.Player1Win },
            { RoundOutcome.Player1Win, RoundOutcome.Player2Win, RoundOutcome.Player1Win, RoundOutcome.Player2Win, RoundOutcome.Draw }
        };

        public bool IsLeaderboardEmpty => !leaderBoard.IsValueCreated || leaderBoard.Value.IsEmpty;

        public GameService()
        {
            PlayerWon += IncreasePlayerScore;
        }

        public IEnumerable<Choice> GetChoices()
        {
            return choices.Value;
        }

        public async Task<Choice> GetRandomChoiceAsync()
        {
            var choiceType = await GetRandomChoiceTypeAsync();

            return new Choice(choiceType);
        }

        public IEnumerable<GameServiceLeaderboardRecordResponse> GetLeaderboard()
        {
            return leaderBoard.IsValueCreated
                ? leaderBoard
                    .Value
                    .Select(x => new GameServiceLeaderboardRecordResponse(x.Key, x.Value))
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Name)
                    .Take(10)
                : Enumerable.Empty<GameServiceLeaderboardRecordResponse>();
        }

        public async Task<GameServiceRoundProcessingResponse> ProcessRound(ChoiceType playerChoice, string? playerName = null)
        {
            var computerChoice = await GetRandomChoiceTypeAsync();

            var outcome = gameLogic[(int)playerChoice - 1, (int)computerChoice - 1];

            if (outcome == RoundOutcome.Player1Win)
            {
                PlayerWon?.Invoke(playerName);
            }

            return new GameServiceRoundProcessingResponse
            {
                Player = playerChoice,
                Computer = computerChoice,
                Results = outcome switch
                {
                    RoundOutcome.Draw => GameServiceRoundProcessingResponse.RoundOutcome.tie,
                    RoundOutcome.Player1Win => GameServiceRoundProcessingResponse.RoundOutcome.win,
                    RoundOutcome.Player2Win => GameServiceRoundProcessingResponse.RoundOutcome.lose
                }
            };
        }

        public void ResetLeaderBoard()
        {
            if (leaderBoard.IsValueCreated)
            {
                leaderBoard.Value.Clear();
            }
        }

        public void MockPlayerWon(string? playerName)
        {
            PlayerWon?.Invoke(playerName);
        }

        private void IncreasePlayerScore(string? playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;
            }

            playerName = playerName.Trim();

            const int PRIZE = 30;

            leaderBoard.Value.AddOrUpdate(playerName, PRIZE, (key, oldValue) => oldValue + PRIZE);
        }

        private async Task<ChoiceType> GetRandomChoiceTypeAsync()
        {
            const int MAGIC_NUMBER = 20;

            var randomServiceResponse = await httpClient
                .Value
                .GetFromJsonAsync<GameServiceRandomGeneratorResponse>(RANDOM_GENERATOR_SERVICE_URI);

            if (randomServiceResponse is null)
            {
                throw new OurUserFriendlyException("Random API is not available.");
            }

            if (!(randomServiceResponse.RecordNumber is >= 0 and <= 100))
            {
                throw new OurUserFriendlyException("Random API is not working properly.");
            }

            var choiceType = (ChoiceType)(Math.Min(99, randomServiceResponse.RecordNumber) / MAGIC_NUMBER) + 1;

            return choiceType;
        }

        private static Choice[] GetChoicesInitialValues()
        {
            return Enum
                .GetValues(typeof(ChoiceType))
                .Cast<ChoiceType>()
                .Select(x => new Choice((int)x, x.ToString()))
                .ToArray();
        }
    }
}
