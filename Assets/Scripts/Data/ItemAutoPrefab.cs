#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CustomEditor(typeof(Item))]
public class ItemAutoPrefab : Editor
{
    private const string PREFAB_FOLDER = "Assets/Prefabs/Items/";
    private const string ITEM_FOLDER = "Assets/Items";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Generate World Prefab"))
        {
            CreateWorldPrefab((Item)target);
        }
    }

    private static int GetNextAvailableId()
    {
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { ITEM_FOLDER });
        HashSet<int> usedIds = new HashSet<int>();

        foreach (string guid in guids)
        {
            Item itm = AssetDatabase.LoadAssetAtPath<Item>(AssetDatabase.GUIDToAssetPath(guid));
            if (itm != null && itm.id >= 0)
                usedIds.Add(itm.id);
        }

        int next = 0;
        while (usedIds.Contains(next)) next++;
        return next;
    }

    private static void CreateWorldPrefab(Item item)
    {
        if (item.id < 0) //it starts at -1(unasigned)
        {
            item.id = GetNextAvailableId();
            EditorUtility.SetDirty(item);
            Debug.Log($"Assigned ID {item.id} to {item.name}", item);
        }

        // 1. Ensure a folder exists 
        if (!AssetDatabase.IsValidFolder(PREFAB_FOLDER.TrimEnd('/')))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Items");

        // 2. Create an empty global object
        GameObject go = new GameObject(item.itemName + "_World");
        Undo.RegisterCreatedObjectUndo(go, "Create item prefab");

        // 3. Add basic visuals
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        // 4. Add required components
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        go.layer = interactableLayer;
        go.AddComponent<BoxCollider>();
        Rigidbody rb = go.AddComponent<Rigidbody>();
        ItemPickup ip = go.AddComponent<ItemPickup>();

        ip.item = item;

        // 5. Save as prefab asset 
        string path = PREFAB_FOLDER + go.name + ".prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);

        // 6. Assign into the Item SO
        item.worldPrefab = prefab;
        EditorUtility.SetDirty(item);

        // 7. Clean‑up
        DestroyImmediate(go);

        Debug.Log($"Created prefab: {path}", prefab);
    }
}
#endif
