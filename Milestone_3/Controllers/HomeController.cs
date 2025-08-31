using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Filters;
using MinesweeperGame.Models;

namespace MinesweeperGame.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // user must be logged in to start a game
        [SessionAuthorizationFilter]
        public IActionResult StartGame()
        {
            // Shows the form to configure a new game with the size and difficulty
            return View(new GameBoardModel());
        }

        [HttpPost]
        public IActionResult StartGame(GameBoardModel board)
        {
            //Processes the form, create a new board and stores it in the session
            // Converts difficulty string to a percentage value
            int difficultyValue = board.Difficulty switch
            {
                "Easy" => 10,
                "Medium" => 15,
                "Hard" => 20,
                _ => 15
            };

            // Creates a new bgame baord via the Board Class constructor
            Board gameBoard = new Board(board.Size, difficultyValue);

            // Stores the entire game state in the user's session
            string gameBoardJson = Newtonsoft.Json.JsonConvert.SerializeObject(gameBoard);
            HttpContext.Session.SetString("GameBoard", gameBoardJson);

            // Redirect the user to the main game board view
            return RedirectToAction("MineSweeperBoard", "Game");
        }
    }
}
