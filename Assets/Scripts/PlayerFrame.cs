using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrame : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private bool canBeDetected;

    [SerializeField] private float radius;



    // Update is called once per frame
    void Update()
    {
        if (canBeDetected)
            playerController.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, layerMask);
        

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canBeDetected = false;
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            canBeDetected = true;
    }

    
}
