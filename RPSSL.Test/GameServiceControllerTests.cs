using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RPSSL.BL.Services;
using RPSSL.BL.Services.Models;
using RPSSL.Common.Entities;
using RPSSL.Common.Enums;
using RPSSL.Controllers;
using RPSSL.Models;

namespace RPSSL.Test
{
    [TestClass]
    public class GameServiceControllerTests
    {
        ILogger<GameServiceController> logger;
        GameService service;
        GameServiceController controller;

        [TestInitialize]
        public void Init()
        {
            logger = NullLoggerFactory.Instance.CreateLogger<GameServiceController>();
            service = new GameService();
            controller = new GameServiceController(service, logger);
        }

        [TestMethod]
        public void GameServiceController_GetChoices_Success()
        {
            var result = controller.GetChoices();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);

            var successResult = result as ApiResponse<IEnumerable<Choice>>;

            Assert.IsNotNull(successResult);
            Assert.IsTrue(successResult.IsSuccessful);
            Assert.IsNotNull(successResult.Data);
            Assert.AreEqual(5, successResult.Data.Count());
        }

        [TestMethod]
        public async Task GameServiceController_GetChoice_Success()
        {
            var result = await controller.GetChoice();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);

            var successResult = result as ApiResponse<Choice>;

            Assert.IsNotNull(successResult);
            Assert.IsTrue(successResult.IsSuccessful);
            Assert.IsNotNull(successResult.Data);
        }

        [TestMethod]
        public async Task GameServiceController_Play_Success()
        {
            const string expectedLeaderName = "John Doe";

            var result = await controller.Play(new GameServicePlayRequestModel(ChoiceType.Rock, expectedLeaderName));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccessful);

            var successResult = result as ApiResponse<GameServiceRoundProcessingResponse>;

            Assert.IsNotNull(successResult);
            Assert.IsTrue(successResult.IsSuccessful);
            Assert.IsNotNull(successResult.Data);

            var data = successResult.Data;

            Assert.IsNotNull(data.Computer);
            Assert.IsNotNull(data.Player);
            Assert.IsNotNull(data.Results);

            if (data.Results == GameServiceRoundProcessingResponse.RoundOutcome.win)
            {
                Assert.IsTrue(data.Computer == ChoiceType.Scissors || data.Computer == ChoiceType.Lizard);

                var leaderboard = service.GetLeaderboard().ToList();

                Assert.IsNotNull(leaderboard);
                Assert.AreEqual(1, leaderboard.Count);

                var leader = leaderboard.First();

                Assert.AreEqual(expectedLeaderName, leader.Name);
                Assert.AreEqual(30, leader.Score);
            }
            else if (data.Results == GameServiceRoundProcessingResponse.RoundOutcome.lose)
            {
                Assert.IsTrue(data.Computer == ChoiceType.Paper || data.Computer == ChoiceType.Spock);
            }
            else if (data.Results == GameServiceRoundProcessingResponse.RoundOutcome.tie)
            {
                Assert.IsTrue(data.Computer == ChoiceType.Rock);
            }
        }
    }
}