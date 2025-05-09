using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponUsage : MonoBehaviour
{
    private PlayerStats stats;
    private Animator anim;
    private float nextAttackTime;

    public void Init(PlayerStats s)
    {
        stats = s;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!stats || !stats.hasWeapon) return;

        // swing
        if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / stats.swordStats.attackRate;
            anim.SetTrigger("Swing");
            CombatEvents.OnSwing?.Invoke();
        }

        // block and parry
        if (Input.GetMouseButtonDown(1))
        {
            stats.StartParryWindow(); //starts the parry window in player stats 
            anim.SetBool("Blocking", true);
            CombatEvents.OnBlockStart?.Invoke();
        }
        if (Input.GetMouseButtonUp(1)) // block ends
        {
            anim.SetBool("Blocking", false);
            CombatEvents.OnBlockEnd?.Invoke();
            stats.StopBlock();

        }
    }
}
