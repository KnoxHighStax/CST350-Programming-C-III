using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Models;

namespace MinesweeperGame.Controllers
{
    public class HomeController : Controller
    {
        // Method that when called upon will open the main index page
        public IActionResult Index()
        {
            // Returns the home page view
            return View();
        }
    }
}
