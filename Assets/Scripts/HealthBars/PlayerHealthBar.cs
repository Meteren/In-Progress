using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    [SerializeField] PlayerController controller;
    private void Update()
    {
        if (controller.isDead)
        {
            avatarAnimator.SetBool("isDead", controller.isDead);
        }
    }
}
