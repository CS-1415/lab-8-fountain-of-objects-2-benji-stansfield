namespace Lab08;

public record Location(int Row, int Column);
public class Player
{
    public Location? Location { get; set; }
    public int Health { get; set; } = 20;
    public int Defense { get; set; } = 12;
    public bool IsAlive { get; private set; } = true;
    public string CauseOfDeath { get; private set; }
    public int ArrowsRemaining { get; set; } = 5;
    public Player(Location start) => Location = start;
    public void Die(string cause)
    {
        CauseOfDeath = cause;
        Console.WriteLine($"{CauseOfDeath}");
        IsAlive = false;
    }
}