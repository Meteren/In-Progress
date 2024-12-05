using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour, ICastSpell
{
    protected float damage { get; set; }
    protected Vector2 damageDircetion { get; set; }
    protected AnimatorStateInfo stateInfo { get; set; }
    [SerializeField] protected Animator animator;
    public virtual void CastSpell(PlayerController controller)
    {
        return;
    }

    protected void DestroySpell(string stateName)
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(stateName))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void InitSpell()
    {
        return;
    }

    public virtual float InflictDamage()
    {
        return damage;
    }
}
