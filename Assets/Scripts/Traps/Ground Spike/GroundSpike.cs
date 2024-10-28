using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpike : MainTrap, ITrap
{
   
    public void TrapLogic()
    {
        controller.knockBack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
            TrapLogic();
    }
}
