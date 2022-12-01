using Microsoft.AspNetCore.Mvc;
using RPSSL.Common.Entities;
using RPSSL.Models;
using RPSSL.BL.Services;
using RPSSL.BL.Exceptions;
using RPSSL.BL.Services.Models;

namespace RPSSL.Controllers
{
    //[Route("api/[controller]")] to comply the requirements
    [Route("")]
    [ApiController]
    public class GameServiceController : BaseApiController
    {
        private GameService gameService;

        public GameServiceController (GameService gameService, ILogger<GameServiceController> logger) : base(logger)
        {
            this.gameService = gameService;
        }

        [HttpGet("choice")]
        public async Task<ApiResponseBase> GetChoice()
        {
            try
            {
                return new ApiResponse<Choice>(await gameService.GetRandomChoiceAsync());
            }
            catch(OurUserFriendlyException ufe)
            {
                logger.LogWarning(ufe, "GetChoice");

                return new BadApiResponse(ufe.Message);
            }
            catch(Exception e)
            {
                logger.LogError(e, "GetChoice");

                return new BadApiResponse();
            }
        }

        [HttpGet("choices")]
        public ApiResponseBase GetChoices()
        {
            try
            {
                return new ApiResponse<IEnumerable<Choice>>(gameService.GetChoices());
            }
            catch (OurUserFriendlyException ufe)
            {
                logger.LogWarning(ufe, "GetChoices");

                return new BadApiResponse(ufe.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "GetChoices");

                return new BadApiResponse();
            }
        }

        [HttpPost("play")]
        public async Task<ApiResponseBase> Play(GameServicePlayRequestModel model)
        {
            try
            {
                return new ApiResponse<GameServiceRoundProcessingResponse>(await gameService.ProcessRound(model.Player, model.PlayerName));
            }
            catch (OurUserFriendlyException ufe)
            {
                logger.LogWarning(ufe, "Play");

                return new BadApiResponse(ufe.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Play");

                return new BadApiResponse();
            }
        }
    }
}
