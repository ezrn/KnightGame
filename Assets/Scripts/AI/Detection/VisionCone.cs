using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VisionCone : MonoBehaviour
{
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private LayerMask targetMask, obstacleMask;
    public event Action<Transform> OnPlayerDetected;

    [SerializeField] private MonoBehaviour customFilter; // drag any script
    IVisionFilter filter;

    void Awake() => filter = customFilter as IVisionFilter;

    public void AdjustRadius(float multiplier) => viewRadius *= multiplier;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach (var hit in hits)
        {
            Vector3 dir = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dir) < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, hit.transform.position, obstacleMask))
                {
                    if (filter == null || filter.CanSee(transform, hit.transform))
                    {
                        OnPlayerDetected?.Invoke(hit.transform);
                    }
                    break;
                }
            }
        }
    }
}
