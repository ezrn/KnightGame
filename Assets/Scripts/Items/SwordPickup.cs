using UnityEngine;

public class SwordPickup : MonoBehaviour, IInteractable
{
    [Header("Sword prefab shown in hand")]
    [SerializeField] private GameObject swordHandPrefab;

    public void Interact(PlayerStats player)
    {
        if (!player) return;

        // Force re‑equip (destroy old instance)
        var equip = player.GetComponent<WeaponEquipController>();
        if (!equip)
            equip = player.gameObject.AddComponent<WeaponEquipController>();

        equip.EquipSword(swordHandPrefab, player);

        // (Optional) play sound, VFX, disable pickup in world
        Destroy(gameObject);
    }
}
