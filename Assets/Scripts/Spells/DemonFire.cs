using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFire : Spell
{
    private void Start()
    {
        damage = 5f;
    }

    private void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        DestroySpell("demonFire");
    }
    public override void CastSpell(PlayerController controller)
    {
        controller.OnDamage(InflictDamage());
    }

    public override float InflictDamage()
    {
        return damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            CastSpell(controller);
        }
    }


}
