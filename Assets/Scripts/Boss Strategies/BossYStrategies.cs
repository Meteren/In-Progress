using System.Collections;
using UnityEngine;

public abstract class MainStrategyForBossY : MainBossStrategy
{
    protected BossY bossY => 
        GameManager.instance.blackBoard.GetValue("BossY", out BossY _bossY) ? _bossY : null;
}


public class CloseRangeAttackStrategyForBossY : MainStrategyForBossY, IStrategy
{
    public CloseRangeAttackStrategyForBossY(bool isBelong = false)
    {
        belongsSpecialAttackBranch = isBelong;
    }
    public Node.NodeStatus Evaluate()
    {

        Debug.Log("Close Range attack");

        if (!bossY.isInCloseRangeAttack)
        {
            bossY.isInCloseRangeAttack = true;

        }
        AnimatorStateInfo stateInfo = bossY.bossAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("CloseRangeAttack"))
        {

            if (stateInfo.normalizedTime >= 1f)
            {
                bossY.isInCloseRangeAttack = false;

                /*if (bossY.specialOneCoroutineBlocker && belongsSpecialAttackBranch)
                    bossY.specialOneCoroutineBlocker = false;*/

                //bossY.probOfSpecialOneAttack = 0;
                return Node.NodeStatus.SUCCESS;
            }
            else
            {
                return Node.NodeStatus.RUNNING;
            }

        }
        else
        {
            return Node.NodeStatus.RUNNING;
        }
    }

}

public class LongRangeAttackStrategyForBossY : MainStrategyForBossY, IStrategy
{
    public LongRangeAttackStrategyForBossY(bool isBelong = false)
    {
        belongsSpecialAttackBranch = isBelong;
    }
    public Node.NodeStatus Evaluate()
    {
        //bossY.rb.velocity = Vector2.zero;
        Debug.Log("Long Range attack");
        if (!bossY.isInLongRangeAttack)
        {
            bossY.isInLongRangeAttack = true;

        }

        AnimatorStateInfo stateInfo = bossY.bossAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("LongRangeAttack"))
        {
            if (stateInfo.normalizedTime >= 1)
            {
                /*if (bossY.specialOneCoroutineBlocker && belongsSpecialAttackBranch)
                    bossY.specialOneCoroutineBlocker = false;*/
                bossY.isInLongRangeAttack = false;
                //bossY.probOfLongRangeAttack = 0;
                return Node.NodeStatus.SUCCESS;
            }
            else
            {
                return Node.NodeStatus.RUNNING;
            }
        }
        else
        {
            return Node.NodeStatus.RUNNING;
        }

    }
}


public class GetCloseStrategy : MainStrategyForBossY, IStrategy
{
    float force = 4f;
    bool previousLocSpotted = false;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("GetCloseStrategy");
        if (!previousLocSpotted)
        {
            bossY.previousLocation = bossY.transform.position;
            previousLocSpotted = true;
        }
            
        bossY.rb.AddForce(new Vector2(bossY.direction.x * force, bossY.direction.y * force),ForceMode2D.Impulse);
        if(bossY.distanceToPlayer <= 2f)
        {
            bossY.rb.velocity = Vector2.zero;
            previousLocSpotted = false;
            return Node.NodeStatus.SUCCESS;
        }
        return Node.NodeStatus.RUNNING;
        
    }
}

public class GetAwayStrategy : MainStrategyForBossY, IStrategy
{
    float speed = 10f;
    float distance = 0f;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("GetAwayStrategy");
        bossY.transform.position = Vector2.MoveTowards(bossY.transform.position, bossY.previousLocation, Time.deltaTime * speed);

        if(Vector2.Distance(bossY.transform.position,bossY.previousLocation) <= distance)
        {
            bossY.rb.velocity = Vector2.zero;
            return Node.NodeStatus.SUCCESS;
        }
        else
        {
            return Node.NodeStatus.RUNNING;
        }
    }
}