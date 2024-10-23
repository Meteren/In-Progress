using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossXHealthBar : HealthBar
{
    [SerializeField] private BossX bossX;

    private void Update()
    {
        if (bossX.canAvatarDie)
        {
            avatarAnimator.SetBool("isDead", bossX.canAvatarDie);
        }
    }
}
