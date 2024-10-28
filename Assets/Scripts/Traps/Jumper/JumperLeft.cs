using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperLeft : MonoBehaviour
{
    [SerializeField] float radius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
