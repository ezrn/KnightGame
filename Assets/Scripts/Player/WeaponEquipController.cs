using UnityEngine;

public class WeaponEquipController : MonoBehaviour
{
    [SerializeField] private Transform orientation; //direction the player is looking
    private GameObject swordInstance;

    public void EquipSword(GameObject prefab, PlayerStats stats)
    {
        // remove existing sword if there is one(for resetting purposes)
        if (swordInstance) Destroy(swordInstance);

        stats.hasWeapon = true; //flag for future use

        // spawn & parent the sword + offset to put in "hand"
        swordInstance = Instantiate(prefab, orientation);
        swordInstance.transform.localPosition = new Vector3(0.2f, 0f, 0.3f);
        swordInstance.transform.localRotation = Quaternion.identity;
        swordInstance.transform.localScale = Vector3.one * 0.33f;

        // if weaponUsage doesnt exist on the sword object for whatever reason, add it
        var usage = GetComponent<WeaponUsage>();
        if (!usage) usage = gameObject.AddComponent<WeaponUsage>();
        usage.Init(stats);
    }

    private void Update()
    {
        // if has weapon, sword instance, or playerStats is ever null or doesnt exist, destroy the sword(more devtool)
        var stats = GetComponent<PlayerStats>();
        if (stats && !stats.hasWeapon && swordInstance)
            Destroy(swordInstance);
    }
}
