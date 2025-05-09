using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatEvents
{
    // player -> world 
    public static Action OnSwing;                       // when the player left clicks(swings)
    public static Action OnBlockStart, OnBlockEnd;      // tracks when the block starts and ends
    public static Action<IEnemy, float> OnParry;        // when an attack is parried
    public static Action OnBlock;                       // when the player blocks an attack

    // enemy -> world
    public static Action<IEnemy> OnEnemyHeartLost;
}

