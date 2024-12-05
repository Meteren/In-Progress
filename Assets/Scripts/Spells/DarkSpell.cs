using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DarkSpell : Spell
{
    private void Start()
    {
        damage = 5f;
    }
    private void Update()
    {
        DestroySpell("spell");
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
