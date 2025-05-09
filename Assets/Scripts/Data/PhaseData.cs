using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Phase")]
public class PhaseData : ScriptableObject
{
    public AttackData[] attacks;
    public float newPoiseMax;
    public GameObject meshOverride;
}
