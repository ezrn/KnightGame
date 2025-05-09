// Projectile.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20;
    private System.Action<Projectile, Collider> hitCallback;
    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    public void Launch(Vector3 dir, System.Action<Projectile, Collider> cb)
    {
        hitCallback = cb;
        rb.velocity = dir.normalized * speed;
        StartCoroutine(DelayedRecycle(6f));         // fallback recycle
    }

    void OnTriggerEnter(Collider other)
    {
        hitCallback?.Invoke(this, other);
        ProjectilePool.Recycle(this);
    }

    private IEnumerator DelayedRecycle(float t)
    {
        yield return new WaitForSeconds(t);
        if (gameObject.activeSelf) ProjectilePool.Recycle(this);
    }
}
