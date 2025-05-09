using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] public GameObject poiseBarObject;
    [SerializeField] private float maxPoise;

    private float currentPoise;
    private float minPoise = 0;

    public bool IsBlocking { get; private set; }
    public bool IsParrying { get; private set; }
    public Transform CurrentTarget { get; set; }

    public bool hasWeapon = false;

    private Vector3 respawnLocation;
    private PoiseBar poiseBar;

    public WeaponStats swordStats = new WeaponStats //weapon stats for the players ONLY weapon
    {
        damage = 25,
        attackRate = 1.2f,
        blockEfficiency = 0.6f,
        parryWindow = 0.15f, //in seconds
        parryPoiseDamage = 50
    };

    void Start() //setup poise bar, set respawn point to players initial spawn position(change later)
    {
        if (poiseBarObject)
            poiseBar = poiseBarObject.GetComponent<PoiseBar>();

        currentPoise = minPoise;
        poiseBar.SetSliderMax(maxPoise);
        setRespawnLocation(player.transform.position);
    }

    public Vector3 getRespawnLocation()
    {
        return respawnLocation;
    }

    public void setRespawnLocation(Vector3 location)
    {
        respawnLocation = location;
    }

    public float getMaxPoise() // when poise reaches its max, players poise "breaks" allowing them to die
    {
        return maxPoise;
    }

    public float getMinPoise() // min poise essentially works as "full health"
    {
        return minPoise;
    }    

    public float getPoise()
    {
        return currentPoise;
    }

    public void TakeHit(HitData hit)
    {
        float dmg = 0;
        if (IsParrying && !hit.isDangerous) // parry and non dangerous == sucessful parry
        {
            CombatEvents.OnParry?.Invoke(hit.source, swordStats.parryPoiseDamage);
            hit.source?.TakeDamage(swordStats.parryPoiseDamage, true);
            return;
        } else if (IsBlocking) { // if mistimed parry or tried to parry dangerous attack, reduce damage
            CombatEvents.OnBlock?.Invoke();
            dmg = hit.poiseDamage * swordStats.blockEfficiency;
        } else { // if did nothing, take full damage
            dmg = hit.poiseDamage;
        }

        currentPoise += dmg; // update poise
        if (currentPoise > maxPoise)
        {
            currentPoise = maxPoise;
        }

        poiseBar.SetSlider(currentPoise);// update slider
    }

    public void HealPoise(float amount)
    {
        currentPoise -= amount;
        if (currentPoise < minPoise)
        {
            currentPoise = minPoise;
        }

        poiseBar.SetSlider(currentPoise);
    }

    public void setMaxPoise(float poise)
    {
        maxPoise = poise;
    }

    public void setPoise(float poise)
    {
        currentPoise = poise;  
    }

    public void StartParryWindow() //called from right click detection in weaponUsage(need to add internal CD)
    {
        if (IsParrying) return;
        StartCoroutine(ParryCoroutine());
    }

    IEnumerator ParryCoroutine()
    {
        IsParrying = true;
        CombatEvents.OnBlockStart?.Invoke(); // shared cue for parry and block
        yield return new WaitForSeconds(swordStats.parryWindow); //parry timing
        IsParrying = false;
        IsBlocking = true; // even if its a parry, its also a block
    }

    public void StopBlock()
    {
        //what happens when the player stops blocking(TODO). Maybe start parry delay/timer?
    }
}
