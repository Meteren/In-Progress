using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLightning : Spell
{
    private void Start()
    {
        damage = 7f;
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
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            CastSpell(controller);
        }
    }
}
