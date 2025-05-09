using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IVisionFilter
{
    bool CanSee(Transform viewer, Transform target);
}
