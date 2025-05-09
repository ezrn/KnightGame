public struct HitData
{
    public float poiseDamage;   // raw amount before block scaling
    public bool isDangerous;   // cannot parry
    public IEnemy source;        // who struck the player (null if trap, etc.)
}