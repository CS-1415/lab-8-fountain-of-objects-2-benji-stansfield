namespace Lab08;

public class Weapon
{
    public int HitChancePlus { get; set; }
    public int Damage { get; set; }

    public Weapon(int hitChancePlus, int damage)
    {
        HitChancePlus = hitChancePlus;
        Damage = damage;
    }
}

public class ShortSword : Weapon
{
    public ShortSword() : base(5, 4) { }
}