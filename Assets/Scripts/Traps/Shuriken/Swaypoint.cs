using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swaypoint : MonoBehaviour
{
    [SerializeField] private float radius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
