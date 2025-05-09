using UnityEngine;

[System.Serializable]
public struct WeaponStats
{
    public float damage;          //  e.g. 25
    public float attackRate;      //  swings per second (1 / cooldown)
    public float blockEfficiency; //  0‑1: dmg reduction while blocking
    public float parryWindow;     //  seconds of perfect‑parry window (1 = 1s)
    public float parryPoiseDamage;// amount of damage done with a parry
}