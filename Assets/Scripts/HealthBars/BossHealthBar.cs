using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    [SerializeField] private Boss boss;

    private void Update()
    {
        if (boss.canAvatarDie)
        {
            avatarAnimator.SetBool("isDead", boss.canAvatarDie);
        }
    }
}
