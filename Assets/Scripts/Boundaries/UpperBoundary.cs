using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperBoundary : MonoBehaviour
{
    [SerializeField] private float distance;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + distance,transform.position.y));
    }
}
