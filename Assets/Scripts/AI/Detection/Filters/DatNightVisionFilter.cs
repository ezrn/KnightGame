using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightVisionFilter : MonoBehaviour, IVisionFilter
{
    [SerializeField] private bool nightBlind;
    public bool CanSee(Transform viewer, Transform tgt)
    {
        bool isNight = true;
        return nightBlind ? !isNight : true;
    }
}
