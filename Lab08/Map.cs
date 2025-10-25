namespace Lab08;

public class Map
{
    private readonly Room[,] _rooms;
    private readonly Random rand = new Random();
    public int Rows { get; }
    public int Columns { get; }

    public Map(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;

        _rooms = new Room[rows, columns];
        GetRoomType();
    }

    public void GetRoomType()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
                _rooms[r, c] = new Room(RoomType.Normal);
        }

        /*Entrance*/
        _rooms[0, 0] = new Room(RoomType.Entrance);

        /*Fountain Room*/
        if (Rows == 4) _rooms[2, 2] = new Room(RoomType.Fountain);
        else if (Rows == 6) _rooms[3, 3] = new Room(RoomType.Fountain);
        else if (Rows == 8) _rooms[4, 4] = new Room(RoomType.Fountain);

        /*Pit Rooms*/
        int numPits = Rows switch { 4 => 1, 6 => 2, 8 => 3, _ => 0 };

        for (int i = 0; i < numPits; i++)
        {
            int row, col;
            do
            {
                row = rand.Next(Rows);
                col = rand.Next(Columns);
            } while (_rooms[row, col].RoomType != RoomType.Normal);

            _rooms[row, col] = new Room(RoomType.Pit);
        }
    }

    public Location GetRandomEmptyLocation()
    {
        int row, col;

        do
        {
            row = rand.Next(Rows);
            col = rand.Next(Columns);
        }
        while (_rooms[row, col].RoomType != RoomType.Normal);

        return new Location(row, col);
    }

    public bool IsAdjacent(Location location, RoomType roomType)
    {
        for (int r = location.Row - 1; r <= location.Row + 1; r++)
        {
            for (int c = location.Column - 1; c <= location.Column + 1; c++)
            {
                // Skip out-of-bounds positions
                if (r < 0 || r >= Rows || c < 0 || c >= Columns)
                    continue;

                // Skip the current room itself
                if (r == location.Row && c == location.Column)
                    continue;

                if (_rooms[r, c].RoomType == roomType)
                    return true;
            }
        }

        return false;
    }

    public bool IsAdjacent(Location a, Location b)
    {
        int rowDiff = Math.Abs(a.Row - b.Row);
        int colDiff = Math.Abs(a.Column - b.Column);
        return rowDiff <= 1 && colDiff <= 1 && !(rowDiff == 0 && colDiff == 0);
    }

    public Room GetRoomAt(Location location) => _rooms[location.Row, location.Column];
}

public class Room
{
    public RoomType RoomType { get; }

    public Room(RoomType roomType)
    {
        RoomType = roomType;
    }
}