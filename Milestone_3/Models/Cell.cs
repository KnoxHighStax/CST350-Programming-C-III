namespace MinesweeperGame.Models
{
    [Serializable]
    public class Cell
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsVisited { get; set; }
        public bool IsLive { get; set; }
        public int LiveNeighbors { get; set; }
        public bool IsFlagged { get; set; }

        public Cell(int id, int row, int column)
        {
            Id = id;
            Row = row;
            Column = column;
            IsVisited = false;
            IsLive = false;
            LiveNeighbors = 0;
            IsFlagged = false;
        }

        public Cell() { }
    }
}
