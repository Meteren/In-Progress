using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainBossStrategy
{
    protected bool blockCoroutine = false;

    protected bool isInProgress = false;

    protected bool belongsSpecialAttackBranch = false;
    protected PlayerController playerController =>
       blackBoard.GetValue("PlayerController", out PlayerController playerController) ? playerController : null;


    protected BlackBoard blackBoard => GameManager.Instance.blackBoard;

}
