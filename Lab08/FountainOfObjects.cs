using System.Xml;

namespace Lab08;

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public Monster[] Monsters { get; }
    public bool IsFountainOn { get; set; }

    public FountainOfObjectsGame(Map map, Player player, Monster[] monsters)
    {
        Map = map;
        Player = player;
        Monsters = monsters;
    }

    public void Run()
    {
        while (!WonGame() && Player.IsAlive)
        {
            DisplayStatus();
            GetSense();
            Console.Write("What would you like to do? ");

            Console.ForegroundColor = ConsoleColor.Green;
            string? playerChoice = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;

            // Checks for what the player wants to do and then executes
            var command = CheckPlayerChoice(playerChoice);
            if (command != null)
                command.Execute(this);
            else Console.WriteLine("You do nothing");

            Room currentRoom = Map.GetRoomAt(Player.Location);

            // Checks for a pitfall
            if (currentRoom.RoomType == RoomType.Pit)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You fell into a pit and took 5 damage.");
                Console.ForegroundColor = ConsoleColor.White;

                Player.Health -= 5;

                if (Player.Health == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Player.Die("You fell into a pit and died on impact.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            // Checks monster encounters
            foreach (Monster monster in Monsters)
            {
                if (monster.IsAlive && monster.Location.Equals(Player.Location))
                {
                    EnemyEncounter(monster);
                }
            }

            Console.ReadKey(true);
        }

        // Runs when the player wins
        if (WonGame())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The fountain of objects has been reactivated and you escaped with your life!");
            Console.ReadKey(true);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Runs when the player dies
        if (Player.IsAlive == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("YOU DIED.");
            Console.ReadKey(true);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public void DisplayStatus()
    {
        Console.WriteLine("---------------------------------------------------------------------------------------------");
        Console.WriteLine($"You have {Player.ArrowsRemaining} arrows remaining.");
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
            "shoot north" => new ShootCommand(Direction.North),
            "shoot south" => new ShootCommand(Direction.South),
            "shoot east" => new ShootCommand(Direction.East),
            "shoot west" => new ShootCommand(Direction.West),
            _ => null
        };
    }

    public void EnemyEncounter(Monster monster)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"You encountered a {monster}");
        Console.ForegroundColor = ConsoleColor.White;

        while (Player.IsAlive && monster.IsAlive)
        {
            if (monster != Snake)
            {
                PlayerTurn(monster);
                EnemyTurn();
            }
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
        else if (Map.IsAdjacent(Player.Location, RoomType.Pit))
        {
            Console.WriteLine("You feel a draft. There is a pit in a nearby room.");
        }
        
        foreach (Monster monster in Monsters)
        {
            if (monster.IsAlive && Map.IsAdjacent(Player.Location, monster.Location))
            {
                if (monster is Amarok)
                    Console.WriteLine("You smell a foul stench. An Amarok is nearby.");
                else if (monster is Maelstrom)
                    Console.WriteLine("You hear the growling and groaning of a Maelstrom in the distance.");
            }
        }

    }
}

public enum RoomType { Normal, Fountain, Entrance, Pit }
public enum Direction { North, South, East, West}
