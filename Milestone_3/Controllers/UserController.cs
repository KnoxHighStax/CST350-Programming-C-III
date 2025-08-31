using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Models;
using ServiceStack.Text;

namespace MinesweeperGame.Controllers
{  
    public class UserController : Controller
    {
        private readonly UserCollection _usersCollection;

        public UserController()
        {
            _usersCollection = new UserCollection();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new RegistrationViewModel());
        }

        // Method to proccess the registration form and the inputted fields
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessRegister(RegistrationViewModel registrationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", registrationViewModel);
            }

            try
            {
                // Check if username already exists
                if (_usersCollection.DoesUsernameExists(registrationViewModel.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    return View("Register", registrationViewModel);
                }

                // Check if email already exists
                if (_usersCollection.DoesEmailExists(registrationViewModel.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View("Register", registrationViewModel);
                }

                // Create new user
                UserModel user = new UserModel
                {
                    FirstName = registrationViewModel.FirstName,
                    LastName = registrationViewModel.LastName,
                    Sex = registrationViewModel.Sex,
                    Age = registrationViewModel.Age,
                    State = registrationViewModel.State,
                    Email = registrationViewModel.Email,
                    Username = registrationViewModel.Username
                };

                user.SetPassword(registrationViewModel.Password);

                int userId = _usersCollection.AddUser(user);

                if (userId > 0)
                {
                    // Get the complete user object from database
                    var completeUser = _usersCollection.GetUserById(userId);

                    // Set the session variable that ProcessLogin expects
                    HttpContext.Session.SetString("User", JsonSerializer.SerializeToString(completeUser));

                    return RedirectToAction("RegistrationSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                    return View("Register", registrationViewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View("Register", registrationViewModel);
            }
        }

        // Method to process the login form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessLogin(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", loginViewModel);
            }

            try
            {
                var userId = _usersCollection.CheckCredentials(loginViewModel.Username, loginViewModel.Password);

                if (userId > 0)
                {
                    var user = _usersCollection.GetUserById(userId);
                    HttpContext.Session.SetString("User", JsonSerializer.SerializeToString(user));
                    return View("LoginSuccess", user);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View("Index", loginViewModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View("Index", loginViewModel);
            }
        }

        // Method to perform a logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RegistrationSuccess()
        {
            return View();
        }

    }
}
