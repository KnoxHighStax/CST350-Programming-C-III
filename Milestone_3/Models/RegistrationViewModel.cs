using System.ComponentModel.DataAnnotations;

namespace MinesweeperGame.Models
{
    public class RegistrationViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Sex { get; set; }
 
        public int Age { get; set; }

        public string State { get; set; }

        public string Email { get; set; }


        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
