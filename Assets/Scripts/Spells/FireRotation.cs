using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRotation : Spell
{
    public bool startRotating = false;
    public bool disappear = false;
    public bool lifeTimeEnded = false;
    

    private void Start()
    {
        damage = 4f;
    }

    private void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimationController();
        if (stateInfo.IsName("disappear"))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                Destroy(gameObject);
            }
        }
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

    private void AnimationController()
    {
        animator.SetBool("startRotating", startRotating);
        animator.SetBool("disappear", disappear);
    }

    public void StartRotating()
    {
        startRotating = true;
        StartCoroutine(LifeTime());
    }

    public void Disappear()
    {
        if (lifeTimeEnded)
        {
            disappear = true;
        }
       
    }

    private IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(4f);
        lifeTimeEnded = true;
    }
}
