using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackFrame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable damageableObject))
        {
            damageableObject.OnDamage();
            if (damageableObject.isVulnerable)
            {
                GetComponentInParent<PlayerController>().punched = true;
            }
            
        }
    }
}
