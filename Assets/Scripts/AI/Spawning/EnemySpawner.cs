using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase prefab;
    private EnemyBase current;

    void Awake() => Spawn();

    private void Spawn()
    {
        current = Instantiate(prefab, transform.position, transform.rotation, transform);
    }

    public static IEnumerator Respawn(EnemyBase dead) //respawn enemies when called
    {
        EnemySpawner sp = dead.GetComponentInParent<EnemySpawner>();
        if (sp == null) yield break;

        bool done = false;
        void Handler() => done = true;
        DeathScreen.OnPlayerRespawned += Handler;

        while (!done) yield return null;      // wait until player respawns

        DeathScreen.OnPlayerRespawned -= Handler;
        sp.Spawn();
    }

}

