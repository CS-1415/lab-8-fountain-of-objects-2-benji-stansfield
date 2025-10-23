namespace Lab08;

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public bool IsFountainOn { get; set; }

    public FountainOfObjectsGame(Map map, Player player)
    {
        Map = map;
        Player = player;
    }

    public void Run()
    {
        while (!WonGame() && Player.IsAlive)
        {
            DisplayStatus();
            GetSense();
            Console.Write("What would you like to do? ");
            string? playerChoice = Console.ReadLine();

            var command = CheckPlayerChoice(playerChoice);
            if (command != null)
                command.Execute(this);
            else Console.WriteLine("You do nothing");
        }

        if (WonGame())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The fountain of objects has been reactivated and you escaped with your life!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public void DisplayStatus()
    {
        Console.WriteLine("---------------------------------------------------------------------------------------------");
        Console.WriteLine($"You are in the room at (Row = {Player?.Location?.Row}, Column = {Player?.Location?.Column}).");
    }
    public bool WonGame()
    {
        if (IsFountainOn == true && Player.Location.Equals(new Location(0, 0)))
            return true;
        return false;
    }

    public ICommand? CheckPlayerChoice(string? choice)
    {
        if (choice == null) return null;

        return choice.ToLower() switch
        {
            "move north" => new MoveCommand(Direction.North),
            "move south" => new MoveCommand(Direction.South),
            "move east" => new MoveCommand(Direction.East),
            "move west" => new MoveCommand(Direction.West),
            "enable fountain" => new EnableFountainCommand(),
            _ => null
        };
    }

    public void GetSense()
    {
        Room currentRoom = Map.GetRoomAt(Player.Location);

        if (currentRoom.RoomType == RoomType.Fountain)
        {
            if (IsFountainOn)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {   
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        else if (currentRoom.RoomType == RoomType.Entrance)
        {
            Console.WriteLine("You see light coming from the cavern entrance.");
        }
    }
}

public record Location (int Row, int Column);
public class Player
{
    public Location? Location { get; set; }
    public bool IsAlive { get; private set; } = true;
    public Player(Location start) => Location = start;
}

public class Map
{
    private readonly Room[,] _rooms;
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

        _rooms[0, 0] = new Room(RoomType.Entrance);
        _rooms[2, 2] = new Room(RoomType.Fountain);
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

public interface ICommand
{
    void Execute(FountainOfObjectsGame game);
}

public class MoveCommand : ICommand
{
    public Direction Direction { get; }

    public MoveCommand(Direction direction)
    {
        Direction = direction;
    }

    public void Execute(FountainOfObjectsGame game)
    {
        Location? currentLocation = game.Player.Location;
        Location? newLocation = Direction switch
        {
            Direction.North => new Location(currentLocation.Row - 1, currentLocation.Column),
            Direction.South => new Location(currentLocation.Row + 1, currentLocation.Column),
            Direction.East => new Location(currentLocation.Row, currentLocation.Column + 1),
            Direction.West => new Location(currentLocation.Row, currentLocation.Column - 1),
            _ => currentLocation
        };

        if (newLocation.Row < 0 || newLocation.Row >= game.Map.Rows || newLocation.Column < 0 || newLocation.Column >= game.Map.Columns)
        {
            Console.WriteLine("You run into a wall.");
            return;
        }

        game.Player.Location = newLocation;
    }
}

public class EnableFountainCommand : ICommand
{
    public void Execute(FountainOfObjectsGame game)
    {
        Room currentRoom = game.Map.GetRoomAt(game.Player.Location);

        if (currentRoom.RoomType == RoomType.Fountain && game.IsFountainOn == false)
        {
            game.IsFountainOn = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("You successfully activate the Fountain of Objects!");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (game.IsFountainOn == true) Console.WriteLine("The Fountain of Objects is already running.");
        else Console.WriteLine("You must find the Fountain of Objects first.");
    }
}

public enum RoomType { Normal, Fountain, Entrance }
public enum Direction { North, South, East, West}
