using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulnerablePoint : Point, IDamageable
{
    public bool isVulnerable {
        get; set; }

    public void OnDamage()
    {
        if (isVulnerable)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            controller.interaction = true;
            UIController.instance.ActivateKey();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            controller.interaction = false;
            UIController.instance.DiasbleKey();
        }
            
    }
    private void Update()
    {
        Debug.Log(isVulnerable);
    }

}
