using System.IO.Compression;
using System.Xml;

namespace Lab08;

public class FountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    public Monster[] Monsters { get; }
    public bool IsFountainOn { get; set; }
    public bool InCombat { get; set; } = false;
    private static readonly Random Dice = new Random();
    public int SavingThrow { get; private set; } = 10;

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

                if (Player.Health <= 0)
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
                    monster.Activate(this);
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
            Player.Die();
        }
    }

    public void DisplayStatus()
    {
        Console.WriteLine("---------------------------------------------------------------------------------------------");
        Console.WriteLine($"Health remaining: {Player.Health} hit points.            Current weapon: {Player.EquippedWeapon}");
        Console.WriteLine($"You are in the room at (Row = {Player?.Location?.Row}, Column = {Player?.Location?.Column}).");
    }
    public bool WonGame() => IsFountainOn && Player.Location.Equals(new Location(0, 0));

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

        InCombat = true;

        while (Player.IsAlive && monster.IsAlive && InCombat)
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

        if (Player.IsAlive == false)
        {
            InCombat = false;
            Player.Die("You were defeated in combat.");
        }
        else
        {
            InCombat = false;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You defeated the monster!");
            Console.ForegroundColor = ConsoleColor.White;
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
            int hit = roll + Player.EquippedWeapon.HitChancePlus;
            int damageRoll = Dice.Next(1, 7);
            int damage = damageRoll + Player.EquippedWeapon.Damage;

            // Natural 1
            if (roll == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You miss big time! You take 2 damage out of pure embarrassment.");
                Player.Health -= 2;

                if (Player.Health <= 0) Player.IsAlive = false;

                return;
            }

            // Natural 20
            if (roll == 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("It's a critical hit!");
                damage = (damageRoll * 2) + Player.EquippedWeapon.Damage;

                Console.WriteLine($"You deal {damage} damage!");
                Console.ForegroundColor = ConsoleColor.White;
                monster.Health -= damage;

                if (monster.Health <= 0) monster.IsAlive = false;
            }

            // Hit
            else if (hit >= monster.ArmorClass)
            {
                Console.WriteLine($"You hit the {monster.GetType().Name}!");
                damage = damageRoll + Player.EquippedWeapon.Damage;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You deal {damage} damage!");
                Console.ForegroundColor = ConsoleColor.White;
                monster.Health -= damage;

                if (monster.Health <= 0) monster.IsAlive = false;
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
            int roll = Dice.Next(1, 21);
            if (roll >= SavingThrow)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You successfully escaped from the monster");
                Console.ForegroundColor = ConsoleColor.White;
                InCombat = false;
                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You could not outrun the monster.");
                Console.ForegroundColor = ConsoleColor.White;
                SavingThrow += 2;
            }
        }
        else Console.WriteLine("You hesitated and lost your turn.");
        Console.ReadKey(true);
    }

    public void EnemyTurn(Monster monster)
    {
        int roll = Dice.Next(1, 21);
        int hit = roll + monster.EquippedWeapon.HitChancePlus;

        if (hit >= Player.ArmorClass)
        {
            int damageRoll = Dice.Next(1, 7);
            int damage = damageRoll + monster.EquippedWeapon.Damage;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"You were attacked by the {monster.GetType().Name}!");
            Console.WriteLine($"You took {damage} damage.");
            Console.ForegroundColor = ConsoleColor.White;
            Player.Health -= damage;

            if (Player.Health <= 0) Player.IsAlive = false;
        }
        Console.ReadKey(true);
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
                else Console.WriteLine("You hear the hissing of a snake in a nearby room.");
            }
        }

    }
}

public enum RoomType { Normal, Fountain, Entrance, Pit }
public enum Direction { North, South, East, West }
