namespace Lab08;

public record Location(int Row, int Column);
public class Player
{
    public Location? Location { get; set; }
    public int Health { get; set; } = 20;
    public int ArmorClass { get; set; } = 12;
    public bool IsAlive { get; private set; } = true;
    public string CauseOfDeath { get; private set; }
    public Weapon EquippedWeapon { get; set; }
    public Player(Location start)
    {
        Location = start; // starting location at the entrance
        EquippedWeapon = new ShortSword(); // default weapon
    }
    public void Die(string cause)
    {
        CauseOfDeath = cause;
        Console.WriteLine($"{CauseOfDeath}");
        IsAlive = false;
    }
}