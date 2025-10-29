using System.IO.Compression;
using System.Xml;

namespace Lab08;

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public Monster[] Monsters { get; }
    public bool IsFountainOn { get; set; }
    private static readonly Random Dice = new Random();

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

    public void EnemyEncounter(Monster monster)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"You encountered a {monster.GetType().Name}");
        Console.ForegroundColor = ConsoleColor.White;

        while (Player.IsAlive && monster.IsAlive)
        {
            if (monster is Snake)
            {
                EnemyTurn(monster);
                if (Player.IsAlive) PlayerTurn(monster);
            }
            else
            {
                PlayerTurn(monster);
                if (monster.IsAlive) EnemyTurn(monster);
            }
        }
    }

    public void PlayerTurn(Monster monster)
    {
        Console.WriteLine("It's your turn!");
        Console.Write("What would you like to do? (attack or run?): ");
        Console.ForegroundColor = ConsoleColor.Green;
        string action = Console.ReadLine()?.ToLower();
        Console.ForegroundColor = ConsoleColor.White;

        if (action == "attack")
        {
            int roll = Dice.Next(1, 21);
            int Hit = roll + Player.EquippedWeapon.HitChancePlus;
            int DamageRoll = Dice.Next(1,7);
            int Damage = DamageRoll + Player.EquippedWeapon.Damage;
            
            // Natural 1
            if (roll == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You miss big time! You take 2 damage out of pure embarrassment.");
                Player.Health -= 2;
                return;
            }

            // Natural 20
            if (roll == 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("It's a critical hit!");
                Damage = (DamageRoll * 2) + Player.EquippedWeapon.Damage;

                Console.WriteLine($"You deal {Damage} damage!");
                Console.ForegroundColor = ConsoleColor.White;
                monster.Health -= Damage;
            }

            // Hit
            else if (Hit >= monster.ArmorClass)
            {
                Console.WriteLine($"You hit the {monster.GetType().Name}!");
                Damage = DamageRoll + Player.EquippedWeapon.Damage;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You deal {Damage} damage!");
                Console.ForegroundColor = ConsoleColor.White;
                monster.Health -= Damage;
            }

            // Miss
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Your attack missed.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        else if (action == "run")
        {
            int savingThrow = 10;
            int roll = Dice.Next(1, 21);
            if (roll >= savingThrow)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You successfully escaped from the monster");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You could not outrun the monster.");
                Console.ForegroundColor = ConsoleColor.White;
                savingThrow += 2;
            }
        }
        else Console.WriteLine("You hesitated and lost your turn.");
    }

    public void EnemyTurn(Monster monster)
    {
        
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
public enum Direction { North, South, East, West }
