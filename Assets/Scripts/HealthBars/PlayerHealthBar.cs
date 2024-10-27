using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    [SerializeField] PlayerController controller => 
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    private void Update()
    {
        if (controller.isDead)
        {
            avatarAnimator.SetBool("isDead", controller.isDead);
        }
    }
}
