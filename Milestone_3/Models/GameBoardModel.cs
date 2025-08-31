using System.ComponentModel.DataAnnotations;

namespace MinesweeperGame.Models
{
    public class GameBoardModel
    {
        [Required(ErrorMessage = "Size is required")]
        [Range(8, 12, ErrorMessage = "Size must be between 8 and 12")]
        public int Size { get; set; } = 10;

        [Required(ErrorMessage = "Difficulty is required")]
        public string Difficulty { get; set; } = "Medium";
    }
}
