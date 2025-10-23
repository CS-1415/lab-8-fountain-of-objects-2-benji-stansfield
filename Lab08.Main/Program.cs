namespace Lab08;

public class Program
{
    static void Main()
    {
        Console.Clear();
        Map map = new Map(4, 4);
        Player player = new Player(new Location(0, 0));
        FountainOfObjectsGame game = new FountainOfObjectsGame(map, player);
        game.Run();
    }
}
