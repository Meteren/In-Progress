using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MainTrap, ITrap
{
    float timer = 1.5f;
    Animator spearAnimator;

    private void Start()
    {
        spearAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            spearAnimator.SetTrigger("triggerSpear");
            timer = 1.5f;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
            TrapLogic();
    }

    public void TrapLogic()
    {
        controller.knockBack = true;
    }
}
