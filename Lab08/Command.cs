namespace Lab08;

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

public class ShootCommand : ICommand
{
    public Direction Direction { get; }

    public ShootCommand(Direction direction)
    {
        Direction = direction;
    }

    public void Execute(FountainOfObjectsGame game)
    {
        if (game.Player.ArrowsRemaining == 0)
        {
            Console.WriteLine("You are out of arrows.");
            return;
        }

        Location currentLocation = game.Player.Location;
        Location targetLocation = Direction switch
        {
            Direction.North => new Location(currentLocation.Row - 1, currentLocation.Column),
            Direction.South => new Location(currentLocation.Row + 1, currentLocation.Column),
            Direction.West => new Location(currentLocation.Row, currentLocation.Column - 1),
            Direction.East => new Location(currentLocation.Row, currentLocation.Column + 1)
        };

        bool foundSomething = false;
        foreach (Monster monster in game.Monsters)
        {
            if (monster.Location == targetLocation && monster.IsAlive)
            {
                monster.IsAlive = false;
                foundSomething = true;
            }
        }

        if (foundSomething)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Your arrow killed a monster!");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else Console.WriteLine("You fire an arrow and missed.");

        game.Player.ArrowsRemaining--;
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