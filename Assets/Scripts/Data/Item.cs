using UnityEngine;
public enum ItemType
{
    Consumable,
    Material
}

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    [HideInInspector] public int id = -1;
    public string itemName;
    public Sprite icon;

    public ItemType type;

    [TextArea]
    public string description;

    public GameObject worldPrefab;
}
