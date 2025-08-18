using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Filters;

namespace MinesweeperGame.Controllers
{
    // Restricting access to logged in users only
    [SessionAuthorizationFilter]
    public class GameController : Controller
    {
        // Method that when called upon will start the game view
        public IActionResult StartGame()
        {
            // Returns the Start Game view
            return View();
        }
    }
}
