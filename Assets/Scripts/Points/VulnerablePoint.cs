using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VulnerablePoint : Point, IDamageable
{
    public bool isVulnerable {
        get; set; }

    [SerializeField] private ParticleSystem particle;

    public void OnDamage()
    {
        if (isVulnerable)
        {
            particle.transform.position = GetComponentInParent<Transform>().transform.position;
            particle.Play();
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (controller.isDead)
            {
                controller.interaction = false;
                UIController.instance.DiasbleKey();
            }
        }
    } 

}
