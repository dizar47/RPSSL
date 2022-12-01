using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPSSL.BL.Services;
using RPSSL.BL.Services.Models;

namespace RPSSL.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly ILogger<LeaderboardModel> logger;
        private readonly GameService gameService;
        public List<GameServiceLeaderboardRecordResponse> Records { get; private set; }

        public LeaderboardModel(ILogger<LeaderboardModel> logger, GameService gameService)
        {
            this.logger = logger;
            this.gameService = gameService;

            Records = gameService.GetLeaderboard().ToList();
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostResetAsync()
        {
            gameService.ResetLeaderBoard();

            return RedirectToPage();
        }
    }
}