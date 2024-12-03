
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MainStrategyForBossZ : MainBossStrategy
{
    protected BossZ bossZ =>
        GameManager.instance.blackBoard.GetValue("BossZ", out BossZ _bossZ) ? _bossZ : null;
}


public class HandleMovementStrategy : MainStrategyForBossZ, IStrategy
{
    public enum State
    {
        moveToChoosedPos,
        moveToPlayerOffset
    }

    State state;
    float rightOffsetDistance =>
        Vector2.Distance(new Vector2(playerController.offset.transform.position.x, 0),
            new Vector2(playerController.transform.position.x,0));
    Vector2 playerOffsetPosition => (playerController.transform.position.x < bossZ.centerPoint.transform.position.x) ?
        (playerController.isFacingRight ? playerController.right.transform.position : playerController.left.transform.position)
        : (playerController.isFacingRight ? playerController.left.transform.position : playerController.right.transform.position);
               
    
    Vector2 choosedPosition;
    bool isPositionChoosed = false;
    float speed;
    float distance = 0.1f;

    public HandleMovementStrategy(State state,Vector2 moveTo,float speed)
    {
        this.state = state;
        this.choosedPosition = moveTo;
        this.speed = speed;
    }
    public Node.NodeStatus Evaluate()
    {
        
        if(state == State.moveToPlayerOffset)
        {
            choosedPosition = playerOffsetPosition;
        }      

        bossZ.transform.position = Vector2.MoveTowards(bossZ.transform.position, choosedPosition, Time.deltaTime * speed);
        if (Vector2.Distance(bossZ.transform.position, choosedPosition) <= distance)
        {
            isPositionChoosed = false;
            return Node.NodeStatus.SUCCESS;
        }

        return Node.NodeStatus.RUNNING;
    }

}

public class MeleeAttackStrategy : MainStrategyForBossZ, IStrategy
{
    float distance = 1f;
    float forcePower = 3f;
    bool getAway = false;
    
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Distance: " + Vector2.Distance(new Vector2(bossZ.transform.position.x, 0),
            new Vector2(playerController.transform.position.x, 0)));
        AnimatorStateInfo stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        bossZ.canMeleeAttack = true;

        if (stateInfo.IsName("melee_attack"))
        {
            if (stateInfo.normalizedTime >= 1)
            {
                bossZ.canMeleeAttack = false;
                getAway = false;
                blockCoroutine = false;
                return Node.NodeStatus.SUCCESS;
            }
            if (Vector2.Distance(new Vector2(bossZ.transform.position.x,0), new Vector2(playerController.transform.position.x,0)) < distance && !getAway && !playerController.isInDash)
            {
                if (!blockCoroutine)
                {
                    blockCoroutine = true;
                    bossZ.StartCoroutine(Timer());
                }
                bossZ.rb.AddForce(new Vector2(bossZ.direction.x < 0 ? forcePower : -1 * forcePower, 0), ForceMode2D.Impulse);
               
                return Node.NodeStatus.RUNNING;
            }

        }
        return Node.NodeStatus.RUNNING;
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.25f);
        bossZ.rb.velocity = Vector2.zero;
        getAway = true;
        blockCoroutine = false;

    }

}



