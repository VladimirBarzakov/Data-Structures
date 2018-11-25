using System;

public class Invader : IInvader
{
    public Invader(int damage, int distance)
    {
        this.Damage=damage;
        this.Distance = distance;
        this.isDeleted = false;
    }
    
    public int Damage { get; set; }
    public int Distance { get; set; }
    public bool isDeleted { get; set; }

    public int CompareTo(IInvader other)
    {
        return other.Damage.CompareTo(this.Damage);
    }
}
