namespace Lab08;

public abstract class Monster
{
    public Location Location { get; set; }
    public abstract int Health { get; set; }
    public abstract int ArmorClass { get; set; }
    public bool IsAlive { get; set; } = true;
    public Monster(Location start) => Location = start;
    public abstract void Activate(FountainOfObjectsGame game);
}

public class Maelstrom : Monster
{
    public Maelstrom(Location start) : base(start) { }
    public override int Health { get; set; } = 20;
    public override int ArmorClass { get; set; } = 9;
    public override void Activate(FountainOfObjectsGame game)
    {
        Console.WriteLine("You encountered a Maelstrom and were swept into another room.");
        game.Player.Location = Clamp(new Location(game.Player.Location.Row - 1, game.Player.Location.Column + 2), game.Map.Rows, game.Map.Columns);
        Location = Clamp(new Location(Location.Row + 1, Location.Column - 2), game.Map.Rows, game.Map.Columns);
    }

    private Location Clamp(Location location, int totalRows, int totalColumns)
    {
        int row = location.Row;
        if (row < 0) row = 0;
        if (row >= totalRows) row = totalRows - 1;

        int column = location.Column;
        if (column < 0) column = 0;
        if (column >= totalColumns) column = totalColumns - 1;

        return new Location(row, column);
    }
}

public class Amarok : Monster
{
    public Amarok(Location start) : base(start) { }
    public override int Health { get; set; } = 10;
    public override int ArmorClass { get; set; } = 15;
    public override void Activate(FountainOfObjectsGame game) => game.Player.Die("You were killed by an Amarok.");
}

public class Snake : Monster
{
    public Snake(Location start) : base(start) { }
    public override int Health { get; set; } = 5;
    public override int ArmorClass { get; set; } = 5;
    public override void Activate(FountainOfObjectsGame game) => game.Player.Die("The snake's venom was too much for you.");
}