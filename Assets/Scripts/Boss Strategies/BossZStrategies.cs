using System.Collections;
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
        Debug.Log("HandleMovementStrategy");
        
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
        Debug.Log("MeleeAttackStrategy");
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

public class DarkSpellStrategy : MainStrategyForBossZ, IStrategy
{
    Spell darkSpell;
    float playerPosYOffset = 1f;
    float playerOffsetPosYOffset = 2f;
    Vector2 playerPosition => playerController.transform.position;
    Vector2 playerOffsetPosition => playerController.offset.transform.position;
    public DarkSpellStrategy(Spell darkSpell)
    {
        this.darkSpell = darkSpell;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DarkSpellStrategy");
        Spell instantiatedDarkSpell = 
            GameObject.Instantiate(darkSpell,
            new Vector2(playerController.rb.velocity != Vector2.zero ? playerOffsetPosition.x : playerPosition.x,
            playerController.rb.velocity != Vector2.zero ? playerOffsetPosition.y + playerOffsetPosYOffset : playerPosition.y + playerPosYOffset),Quaternion.identity);

        return Node.NodeStatus.SUCCESS;
    }
}

public class CastSpellStrategy : MainStrategyForBossZ, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("CastSpellStrategy");
        bossZ.castSpell = true;
        stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("phaseOne_cast_spell"))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                bossZ.castSpell = false;
                return Node.NodeStatus.SUCCESS;
            }
        }
        return Node.NodeStatus.RUNNING;
    }
}

public class CastShieldStrategy : MainStrategyForBossZ, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        bossZ.createShield = true;
        stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("createShield"))
        {
            if (stateInfo.normalizedTime >= 1)
            {
                return Node.NodeStatus.SUCCESS;
            }
        }
        return Node.NodeStatus.RUNNING;
    }
}

public class DemonSpellStrategy : MainStrategyForBossZ, IStrategy
{
    int spellCount = 8;
    Spell instantiatedSpell;
    float increaseOffsetAmount = 2;
    float secondsToWait = 0.3f;

    Vector2 closestWayPoint => 
        (Vector2.Distance(bossZ.demonSpellWayPointOne.transform.position, playerController.transform.position) <
          Vector2.Distance(bossZ.demonSpellWayPointTwo.transform.position, playerController.transform.position)) ? bossZ.demonSpellWayPointOne.transform.position
        :bossZ.demonSpellWayPointTwo.transform.position;
    Vector2 capturedClosestWayPoint;
    public DemonSpellStrategy(Spell instantiatedSpell)
    {
        this.instantiatedSpell = instantiatedSpell;

    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DemonSpellStrategy");
        capturedClosestWayPoint = closestWayPoint;
        SpellLogic();
        return Node.NodeStatus.SUCCESS;
    }

    private void SpellLogic()
    {
        bossZ.StartCoroutine(Cast());
    }

    private IEnumerator Cast()
    {
        Vector2 spellPosition = closestWayPoint;
        bossZ.demonSpellInProgress = true;
        for (int i = 0; i < 10; i++)
        {
            Spell demonFire = GameObject.Instantiate(instantiatedSpell);
            demonFire.transform.position = spellPosition;
            spellPosition = new Vector2(capturedClosestWayPoint.x < bossZ.transform.position.x ? 
                spellPosition.x += increaseOffsetAmount : spellPosition.x -= increaseOffsetAmount, spellPosition.y);
            yield return new WaitForSeconds(secondsToWait);

        }
        bossZ.demonSpellInProgress = false;
    }
}



