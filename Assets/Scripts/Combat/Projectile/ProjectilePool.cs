
// ProjectilePool.cs
using System.Collections.Generic;
using UnityEngine;

public static class ProjectilePool
{
    private static readonly Dictionary<Projectile, Queue<Projectile>> pools
        = new Dictionary<Projectile, Queue<Projectile>>();

    public static Projectile Get(Projectile prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.TryGetValue(prefab, out var q) || q.Count == 0)
        {
            return Object.Instantiate(prefab, pos, rot);
        }
        var p = q.Dequeue();
        p.transform.SetPositionAndRotation(pos, rot);
        p.gameObject.SetActive(true);
        return p;
    }

    public static void Recycle(Projectile instance)
    {
        instance.gameObject.SetActive(false);
        if (!pools.TryGetValue(instance, out var q))
            pools[instance] = q = new Queue<Projectile>();
        q.Enqueue(instance);
    }
}

