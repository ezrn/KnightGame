using UnityEngine;
using System;

public interface IEnemy
{
    Transform Target { get; }
    int HeartsRemaining { get; }

    /// <summary>Damage is applied to the current poise bar.  
    /// If poise hits ≤ 0 an internal “heart” is consumed.</summary>
    void TakeDamage(float amount, bool wasParried);

    /// <summary>Returns which attack animation / projectile to play **next**.</summary>
    AttackData GetNextAttack();

    /// <summary>Fires every time the enemy drops a heart / enters a new phase.</summary>
    event Action<IEnemy> HeartLost;
}