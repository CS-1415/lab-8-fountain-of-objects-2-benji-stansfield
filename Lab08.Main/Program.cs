namespace Lab08;

public class Program
{
    static void Main()
    {
        Console.Clear();

        int size = ChooseMapSize();

        static int ChooseMapSize()
        {
            while (true)
            {
                Console.WriteLine("Choose your map size:");
                Console.WriteLine("1. 4x4");
                Console.WriteLine("2. 6x6");
                Console.WriteLine("3. 8x8");
                Console.Write("Enter your choice (1-3): ");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1": return 4;
                    case "2": return 6;
                    case "3": return 8;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.\n");
                        break;
                }
            }
        }

        Map map = new Map(size, size);
        Player player = new Player(new Location(0, 0));
        Monster[] monsters;

        // Monsters for a small game
        if (map.Rows == 4)
        {
            monsters = new Monster[]
            {
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation())
            };
        }

        // Monsters for a medium game
        else if (map.Rows == 6)
        {
            monsters = new Monster[]
            {
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation()),
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation())
            };
        }

        // Monsters for a large game
        else if (map.Rows == 8)
        {
            monsters = new Monster[]
            {
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation()),
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation()),
                new Amarok(map.GetRandomEmptyLocation()),
                new Maelstrom(map.GetRandomEmptyLocation())
            };
        }

        else monsters = Array.Empty <Monster>();

        FountainOfObjectsGame game = new FountainOfObjectsGame(map, player, monsters);

        Console.Clear();
        Console.WriteLine("You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects.");
        Console.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the caverns. You must navigate the Caverns with your other senses.");
        Console.WriteLine("Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.");
        Console.WriteLine("Maelstroms are violent forces of sentient wind. Entering a room with one could transport you to any other location in the caverns. You will be able to hear their growling and groaning in nearby rooms.");
        Console.WriteLine("Amaroks roam the caverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.");
        Console.WriteLine("You carry with you a bow and a quiver of arrows. You can use them to shoot monsters in the caverns but be warned: you have a limited supply.");
        Console.WriteLine("\nFind the Fountain of Objects, activate it, and return to the entrance.");
        Console.WriteLine("If you accept your quest press any key.");

        Console.ReadKey(true);
        Console.Clear();
        game.Run();
    }
}
