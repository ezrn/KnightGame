using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scripts/Data/AttackData.cs
[CreateAssetMenu(menuName = "Combat/Attack")]
public class AttackData : ScriptableObject
{
    public string animTrigger; // ? 
    public float poiseDamage; //how much damage does it do?
    public bool dangerous; // can it be parried?
    public float range;      // ?
    public Projectile projectilePrefab; // null for melee
}
