using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MinesweeperGame.Data;
using MinesweeperGame.Models;

namespace MinesweeperGame.Controllers
{
    public class UserController : Controller
    {
        // To be able to access the database
        private readonly UserRepository _userRepository;

        // Constructor with a dependency injection
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Method that will display the registration form when called upon
        [HttpGet]
        public IActionResult Register()
        {
            // Return the form back as empty
            return View(new RegistrationViewModel());
        }

        // Method to proccess the registration form and the inputted fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegistrationViewModel model)
        {
            // To validate the model from the form submission
            if (!ModelState.IsValid)
            {
                // Returns form with error
                return View(model);
            }
            try
            {
                // Create a new user from the form data that was inputted
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Sex = model.Sex,
                    Age = model.Age,
                    State = model.State,
                    Email = model.Email,
                    Username = model.Username,
                    Password = model.Password
                };
                // Registers the user to the database
                int userId = _userRepository.RegisterUser(user);
                // Redirect to success page with ne wuser ID
                return RedirectToAction("RegistrationSuccess", new { userId });
            }
            // Error handling
            catch (Exception ex) 
            {
                ModelState.AddModelError("", "Registration failed. Please try Again.");
                return View(model);
            }
        }

        // Method to display registration success page
        public IActionResult RegistrationSuccess(int userId)
        {
            // Pass the user's ID to the main view
            ViewBag.Id = userId;
            return View();
        }

        // Method to display the login form
        [HttpGet]
        public IActionResult Login()
        {
            // Returns the login form back empty
            return View(new LoginViewModel());
        }

        // Method to process the login form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Returns the form with validation errors
                return View(model);
            }

            // Authenticate the user against the database
            var user = _userRepository.AuthenticateUser(model.Username, model.Password);

            // Successful login
            if (user != null)
            {
                // Store the user info in the session
                HttpContext.Session.SetInt32("Id", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                // Will then redirect to games starting page
                return RedirectToAction("StartGame", "Game");
            }
            // If the login has failed
            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        // Method to perform a logout
        public IActionResult Logout()
        {
            // Clears data from session
            HttpContext.Session.Clear();
            // redirects to main home page
            return RedirectToAction("Index", "Home");
        }
    }
}
