using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Filters;
using MinesweeperGame.Models;
using MinesweeperGame.Services;
using Newtonsoft.Json;

namespace MinesweeperGame.Controllers
{
    // Ensuring User u=is logged in for all actions for this controller

    [SessionAuthorizationFilter]
    public class GameController : Controller
    {
        // Dependency injection for the game logic
        private readonly IMinesweeperService _minesweeperService;

        public GameController(IMinesweeperService minesweeperService)
        {
            _minesweeperService = minesweeperService;
        }

        [SessionAuthorizationFilter]
        public IActionResult MineSweeperBoard()
        {
            // Retrieving the serialized game board from the user's session
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                Console.WriteLine("No game board in session - redirecting to StartGame");
                return RedirectToAction("StartGame", "Home");
            }

            try
            {
                // Deserialize teh JSON back into a Board object to pass to the view
                Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);
                return View(gameBoard);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return RedirectToAction("StartGame", "Home");
            }
        }

        [HttpPost]
        public IActionResult CellClick(int row, int col)
        {
            // AJAX - Handles a left-click event on a cell
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                return Json(new { success = false, redirect = Url.Action("StartGame", "Home") });
            }

            try
            {
                Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);

                // Delegates the game logic to the servic e layer
                (var updatedBoard, bool gameEnded) = _minesweeperService.ProcessCellClick(gameBoard, row, col);

                // Save the updated game state back to the session
                HttpContext.Session.SetString("GameBoard", JsonConvert.SerializeObject(updatedBoard));

                // Checking if the click ended the game -- If win or loss
                if (gameEnded)
                {
                    if (updatedBoard.GameOver)
                        return Json(new { success = true, redirect = Url.Action("GameOver", "Game") });
                    else if (updatedBoard.GameWon)
                        return Json(new { success = true, redirect = Url.Action("GameWon", "Game") });
                }
                // If the game is still ongoing, return only the updated cell's HTML
                var cell = updatedBoard.Grid[row, col];

                // Partial view- Sends back HTML for just this cell
                return PartialView("_Cell", cell);
            }
            catch (Exception)
            {
                return Json(new { success = false, redirect = Url.Action("StartGame", "Home") });
            }
        }

        [HttpPost]
        public IActionResult ToggleFlag(int row, int col)
        {
            // AJAX - Handles a right-click event on a cell --  Will toggle a flag image
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                return Json(new { success = false, redirect = Url.Action("StartGame", "Home") });
            }

            try
            {
                Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);

                // Delegates the flag toggling logic to the srvice layer
                var updatedBoard = _minesweeperService.ToggleFlag(gameBoard, row, col);

                HttpContext.Session.SetString("GameBoard", JsonConvert.SerializeObject(updatedBoard));

                // Return the updated cell's HTML for partial page updates
                var cell = updatedBoard.Grid[row, col];
                // Return the Partial view for the user
                return PartialView("_Cell", cell);
            }
            catch (Exception)
            {
                return Json(new { success = false, redirect = Url.Action("StartGame", "Home") });
            }
        }

        public IActionResult GetGameInfo()
        {
            // AJAX -  Fetches the current game stats for updating the info panel -- Placement of flags, mines, timestamps
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                return Content("");
            }

            try
            {
                Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);
                // Partail View - Updates the game info panel
                return PartialView("_GameInfo", gameBoard);
            }
            catch (Exception)
            {
                return Content("");
            }
        }

        public IActionResult GameOver()
        {
            // Displays the game over screen, revealing all mines
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                return RedirectToAction("StartGame", "Home");
            }

            try
            {
                Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);
                return View(gameBoard);
            }
            catch (Exception)
            {
                return RedirectToAction("StartGame", "Home");
            }
        }

        public IActionResult GameWon()
        {
            // Displays the victory screen
            var gameBoardJson = HttpContext.Session.GetString("GameBoard");
            if (string.IsNullOrEmpty(gameBoardJson))
            {
                return RedirectToAction("StartGame");
            }

            Board gameBoard = JsonConvert.DeserializeObject<Board>(gameBoardJson);

            return View(gameBoard);
        }
    }
}
