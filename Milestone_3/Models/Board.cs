namespace MinesweeperGame.Models
{
    // Required to help store the object in the session
    [Serializable]
    public class Board
    {
        public int Size { get; set; }
        public Cell[,] Grid { get; set; }
        public int Difficulty { get; set; }
        public int MineCount { get; set; }
        public int FlagCount { get; set; }
        public bool GameOver { get; set; }
        public bool GameWon { get; set; }

        public Board(int size, int difficulty)
        {
            Size = size;
            Difficulty = difficulty;
            Grid = new Cell[Size, Size];
            FlagCount = 0;
            GameOver = false;
            GameWon = false;

            /// Create all cell objects
            InitializeBoard();
            // Randomly place mines
            SetupLiveNeighbors();
            // Calculate number for each cell -- the amount of live bomb neighbors it will have
            CalculateLiveNeighbors();
        }

        private void CalculateLiveNeighbors()
        {
            // Calculates how many adjacent mines each cell has
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (!Grid[row, col].IsLive)
                    {
                        Grid[row, col].LiveNeighbors = CalculateLiveNeighbors(row, col);
                    }
                    else
                    {
                        Grid[row, col].LiveNeighbors = 0;
                    }
                }
            }
        }

        private void SetupLiveNeighbors()
        {
            // Places mines randomly on the baord based on the difficluty percentage
            int totalCells = Size * Size;
            MineCount = (int)(totalCells * (Difficulty / 100.0));

            Random random = new Random();
            int minesPlaced = 0;

            while (minesPlaced < MineCount)
            {
                int row = random.Next(0, Size);
                int col = random.Next(0, Size);

                if (!Grid[row, col].IsLive)
                {
                    Grid[row, col].IsLive = true;
                    minesPlaced++;
                }
            }
        }

        private int CalculateLiveNeighbors(int row, int col)
        {
            // Helper method to count mines in the 8 surrounding cells for a given cell
            int liveNeighbors = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Skip the cell itself
                    if (i == 0 && j == 0) continue;

                    int neighborRow = row + i;
                    int neighborCol = col + j;

                    // Check if the neighbor is within the bounds and to make sure it is a mine
                    if (neighborRow >= 0 && neighborRow < Size &&
                        neighborCol >= 0 && neighborCol < Size &&
                        Grid[neighborRow, neighborCol].IsLive)
                    {
                        liveNeighbors++;
                    }
                }
            }
            return liveNeighbors;
        }

        private void InitializeBoard()
        {
            // Creates a 2D grid of cell objects
            int id = 0;
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    Grid[row, col] = new Cell(id, row, col);
                    id++;
                }
            }
        }

        public void FloodFill(int row, int col)
        {
            // Recursive algorithm to reveal all adjacent empty cells when an empty cell is clicked
            // Check for out-of-bounds or cells that should'nt be revealed
            if (row < 0 || row >= Size || col < 0 || col >= Size)
                return;

            Cell cell = Grid[row, col];
            if (cell.IsVisited || cell.IsFlagged || cell.IsLive)
                return;

            cell.IsVisited = true;

            // If the cell is empty, reveal its neighbors recursively
            if (cell.LiveNeighbors == 0)
            {
                FloodFill(row - 1, col);     
                FloodFill(row + 1, col);     
                FloodFill(row, col - 1);     
                FloodFill(row, col + 1);     
                FloodFill(row - 1, col - 1); 
                FloodFill(row - 1, col + 1); 
                FloodFill(row + 1, col - 1); 
                FloodFill(row + 1, col + 1); 
            }
        }

        public void RevealAllMines()
        {
            // Reveals all mines on the bord when the game is lost
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    Cell cell = Grid[row, col];
                    if (cell.IsLive)
                    {
                        cell.IsVisited = true;
                    }
                }
            }
        }

        
        public void ToggleFlag(int row, int col)
        {
            // Toggles a flag on a cell and manges the flag count
            Cell cell = Grid[row, col];
            if (!cell.IsVisited)
            {
                cell.IsFlagged = !cell.IsFlagged;
                if (cell.IsFlagged)
                {
                    FlagCount++;
                }
                else
                {
                    FlagCount--;
                }
            }
        }

        // Method to help to to see if the sell is already flagged
        public bool IsCellFlagged(int row, int col)
        {
            return Grid[row, col].IsFlagged;
        }
    }
}
