using MinesweeperGame.Models;

namespace MinesweeperGame.Services
{
    public interface IMinesweeperService
    {
        Board CreateNewGame(int size, int difficulty);
        // Left click-logic
        (Board, bool) ProcessCellClick(Board gameBoard, int row, int col);
        // Right-click logic
        Board ToggleFlag(Board gameBoard, int row, int col);
        bool CheckForWin(Board gameBoard);
    }
    public class MinesweeperService : IMinesweeperService
    {
        public Board CreateNewGame(int size, int difficulty)
        {
            // Methos to creta a new board -- Loigc in board constructor
            return new Board(size, difficulty);
        }

        public (Board, bool) ProcessCellClick(Board gameBoard, int row, int col)
        {
            // Processes a left-click on a cel
            // Check if the game is already over
            if (gameBoard.GameOver || gameBoard.GameWon)
                return (gameBoard, false);

            Cell cell = gameBoard.Grid[row, col];

            // A flagged cell should not respond to the left-click
            if (cell.IsFlagged)
                return (gameBoard, false);

            //If the cell is a mine, game ovver
            if (cell.IsLive)
            {
                gameBoard.GameOver = true;
                // Reveal all mines on the board
                gameBoard.RevealAllMines();
                // Signal that the game has come to an end
                return (gameBoard, true);
            }

            // If the cell is not yet visited then reveal it
            if (!cell.IsVisited)
            {
                if (cell.LiveNeighbors == 0)
                {
                    //If it is an empty cell perform a flood fill to reveal all adjacent cells
                    gameBoard.FloodFill(row, col);
                }
                else
                {
                    // If it's a number reveal the cell and its number
                    cell.IsVisited = true;
                }

                // AFter revealing will checck to see if this action has won the game
                if (CheckForWin(gameBoard))
                {
                    gameBoard.GameWon = true;
                    // Signale that the game has been won
                    return (gameBoard, true);
                }
            }
            // Game will continue playing
            return (gameBoard, false);
        }

        public Board ToggleFlag(Board gameBoard, int row, int col)
        {
            // Right-click to plant or remove a flag
            // Only allows flag togglin if the game is still active
            if (!gameBoard.GameOver && !gameBoard.GameWon)
            {
                // Delegates tha actual toggling and flag count logic to the Board class
                gameBoard.ToggleFlag(row, col);
            }
            return gameBoard;
        }

        public bool CheckForWin(Board gameBoard)
        {
            // All cells are safe and will now be revealed
            int safeCellsRevealed = 0;
            int totalSafeCells = (gameBoard.Size * gameBoard.Size) - gameBoard.MineCount;

            for (int row = 0; row < gameBoard.Size; row++)
            {
                for (int col = 0; col < gameBoard.Size; col++)
                {
                    if (gameBoard.Grid[row, col].IsVisited && !gameBoard.Grid[row, col].IsLive)
                    {
                        safeCellsRevealed++;
                    }
                }
            }

            return (safeCellsRevealed == totalSafeCells);
        }
    }
}
