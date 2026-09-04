using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGizmos : MonoBehaviour
{
    public float radius;

    void OnDrawGizmos()
    {
        if (radius == 0)
        {
            Gizmos.color = Color.red;
            for (int k = 0; k < transform.childCount; k++)
            {
                var a = transform.GetChild(k);
                Gizmos.DrawSphere(a.position, 0.1f);
            }
        }
        else Gizmos.DrawWireSphere(transform.position, radius);
    }
}
