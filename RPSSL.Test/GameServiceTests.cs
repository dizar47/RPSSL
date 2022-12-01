using RPSSL.BL.Services;
using RPSSL.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace RPSSL.Test
{
    [TestClass]
    public class GameServiceTests
    {
        GameService service = new GameService();

        [TestMethod]
        public async Task GameService_GetChoice_Success()
        {
            var result = await service.GetRandomChoiceAsync();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Name);
        }


        [TestMethod]
        public void GameService_GetChoices_Success()
        {
            var result = service.GetChoices();

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public async Task GameService_ProcessRound_Success()
        {
            const string expectedLeaderName = "John Doe";
            const ChoiceType userChoice = ChoiceType.Rock;

            var result = await service.ProcessRound(userChoice, expectedLeaderName);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Computer);
            Assert.IsNotNull(result.Player);
            Assert.IsNotNull(result.Results);

            if (result.Results == BL.Services.Models.GameServiceRoundProcessingResponse.RoundOutcome.win)
            {
                Assert.IsTrue(result.Computer == ChoiceType.Scissors || result.Computer == ChoiceType.Lizard);

                var leaderboard = service.GetLeaderboard().ToList();

                Assert.IsNotNull(leaderboard);
                Assert.AreEqual(1, leaderboard.Count);

                var leader = leaderboard.First();

                Assert.AreEqual(expectedLeaderName, leader.Name);
                Assert.AreEqual(30, leader.Score);
            }
            else if (result.Results == BL.Services.Models.GameServiceRoundProcessingResponse.RoundOutcome.lose)
            {
                Assert.IsTrue(result.Computer == ChoiceType.Paper || result.Computer == ChoiceType.Spock);
            }
            else if (result.Results == BL.Services.Models.GameServiceRoundProcessingResponse.RoundOutcome.tie)
            {
                Assert.IsTrue(result.Computer == ChoiceType.Rock);
            }
        }

        [TestMethod]
        public void GameService_GetLeaderboard_ResetLeaderboard_Success()
        {
            const string expectedLeaderName = "John Doe";

            Assert.IsTrue(service.IsLeaderboardEmpty);

            service.MockPlayerWon(expectedLeaderName);
            service.MockPlayerWon(expectedLeaderName);
            service.MockPlayerWon("Jane Doe");
            service.MockPlayerWon(expectedLeaderName);
            service.MockPlayerWon("Julie Doe");

            Assert.IsFalse(service.IsLeaderboardEmpty);

            var result = service.GetLeaderboard().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            var leader = result.First();

            Assert.AreEqual(expectedLeaderName, leader.Name);
            Assert.AreEqual(90, leader.Score);

            service.ResetLeaderBoard();

            Assert.IsTrue(service.IsLeaderboardEmpty);
        }
    }
}