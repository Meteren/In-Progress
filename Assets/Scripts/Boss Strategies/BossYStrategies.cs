
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class MoveToPointStrategy : MainStrategyForBossY, IStrategy
{
    Transform pointToMove;
    Vector2 movePosition;
    float speed;
    float howFar = 0.1f;
    bool speedSetted = false;
    bool positionSpotted = false;
    bool isMovingUp = false;
    float distance => Vector2.Distance(bossY.transform.position, movePosition);
    public MoveToPointStrategy(Transform pointToMove,float speed, bool isMovingUp)
    {
        this.pointToMove = pointToMove;
        this.speed = speed;
        this.isMovingUp = isMovingUp;
    }
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("MoveToPoint");

        if (!speedSetted)
        {
            bossY.rb.velocity = Vector2.zero;
            speedSetted = true;
        }

        if (!positionSpotted)
        {
            movePosition = pointToMove.position;
            positionSpotted = true;
        }

        bossY.transform.position = Vector2.MoveTowards(bossY.transform.position, movePosition, Time.deltaTime * speed);
        
        if(distance <= howFar)
        {
            speedSetted = false;
            positionSpotted = false;
            return Node.NodeStatus.SUCCESS;
        }
        else
        {
            return Node.NodeStatus.RUNNING;
        }
    }
}

public class SetAttachedSpiritsAroundStrategy : MainStrategyForBossY, IStrategy
{
    float angle = 0;
    float incrementAmount = 45;
    bool spiritsSet;
    SummonedSpirit listenedSpirit;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("SetSpiritStrategy");
        if (!spiritsSet)
        {
            bossY.canSummon = true;
            SetSpirits();
            listenedSpirit = bossY.offenseSpirits.Dequeue();
            spiritsSet = true;
        }

        if (listenedSpirit.spiritStateMachine.currentState is OrbitalMoveState)
        {
            bossY.canSummon = false;
            spiritsSet = false;
            bossY.offenseSpirits.Enqueue(listenedSpirit);
            bossY.activateSpecialOne = false;
            return Node.NodeStatus.SUCCESS;
        }
        else
        {
            return Node.NodeStatus.RUNNING;
        }           
    }
    private void SetSpirits()
    {
        for (int i = 0; i < 8; i++)
        {
            SummonedSpirit defenseSpirit = GameObject.Instantiate(bossY.referenceSpirit);
            SummonedSpirit offenseSpirit = GameObject.Instantiate(bossY.referenceSpirit);
            bossY.offenseSpirits.Enqueue(offenseSpirit);
            bossY.defenseSpirits.Add(defenseSpirit);
            defenseSpirit.Init(bossY.centerPoint, angle * Mathf.Deg2Rad, bossY.centerPoint.GetComponent<WayPoint>().radius, false, true,bossY.generationFrame);
            offenseSpirit.Init(bossY.centerPoint, angle * Mathf.Deg2Rad, 0, true, true,bossY.generationFrame);
            angle += incrementAmount;
        }
    }

}

public class ShootSpiritsStrategy : MainStrategyForBossY, IStrategy
{
    bool isInWaitSituation = false;
    bool finishStage = false;
    AnimatorStateInfo animatorStateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("ShootSpiritStrategy");
        animatorStateInfo = bossY.bossAnim.GetCurrentAnimatorStateInfo(0);
        if (!isInWaitSituation)
        {
            if (bossY.offenseSpirits.Count != 0)
            {
                bossY.summonAttack = true;
                ShootSpirit();
                isInWaitSituation = true;
            }
  
        }

        if (animatorStateInfo.IsName("summonAttack"))
        {
            if (animatorStateInfo.normalizedTime >= 1)
            {
                bossY.summonAttack = false;
                bossY.StartCoroutine(Timer());

            }
        }
            
        if(bossY.offenseSpirits.Count == 0)
        {
            if (!blockCoroutine)
            {
                bossY.StartCoroutine(WaitABit());
                bossY.StartCoroutine(Timer());
                blockCoroutine = true;
            }
            if (finishStage)
            {
                foreach (var spirit in bossY.defenseSpirits)
                {
                    spirit.isAttached = false;
                    spirit.selectRandomPos = true;
                }
                bossY.defenseSpirits.Clear();
                bossY.StartCoroutine(Timer());
                blockCoroutine = false;
                finishStage = false;
                isInWaitSituation = false;
                bossY.summonAttack = false;
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

    private void ShootSpirit()
    {
        
        SummonedSpirit dequeuedSpirit = bossY.offenseSpirits.Dequeue();
        dequeuedSpirit.isAttached = false;
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.4f);
        isInWaitSituation = false;
        bossY.summonAttack = false;
    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(1f);
        finishStage = true;
        
    }
}

public class NeedleAttackStrategy : MainStrategyForBossY, IStrategy
{
    AnimatorStateInfo animatorStateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("NeedleAttackStrategy");
        if (!isInProgress)
        {
            bossY.activateSkill = true;
            isInProgress = true;
        }
           
        animatorStateInfo = bossY.bossAnim.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("NeedleAttack"))
        {
            if(animatorStateInfo.normalizedTime >= 1)
            {
                bossY.activateSkill = false;
                isInProgress = false;
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

public class GetGunAndMoveUpStrategy : MainStrategyForBossY, IStrategy
{
    GameObject lockingPosition;
    float gunDistanceFromBossy = 1f;
    float gunFollowSpeed = 15f;
    float bossYDistanceFromUpperPoint = 0.1f;
    float bossYMoveSpeed = 17f;

    bool isCollisionIgnored = false;
    bool lockGunPosition;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("GetGunAndMoveUpStrategy");
        if (!isCollisionIgnored)
        {
            HandleCollision(true);
            bossY.wideCam.Priority = 11;
            isCollisionIgnored = true;
        }

        if (Vector2.Distance(bossY.transform.position, bossY.upperPoint.transform.position) >= bossYDistanceFromUpperPoint)
        {
            bossY.transform.position =
                Vector2.MoveTowards(bossY.transform.position, bossY.upperPoint.transform.position, Time.deltaTime * bossYMoveSpeed);
        }
        else
        {
            if(lockingPosition is not null)
            {
                if (lockingPosition.transform.IsChildOf(bossY.transform))
                {
                    lockingPosition.transform.SetParent(null);
                    HandleCollision(false);
                    isCollisionIgnored = false;
                    lockGunPosition = false;
                    return Node.NodeStatus.SUCCESS;
                }
            }
            
        }

        if (!lockGunPosition)
        {
            bossY.gun.isBelongToPlayer = false;
            bossY.gun.isPositionSetted = false;

            bossY.gun.transform.position =
                        Vector2.MoveTowards(bossY.gun.transform.position, bossY.transform.position, Time.deltaTime * gunFollowSpeed);
            if (Vector2.Distance(bossY.gun.transform.position, bossY.transform.position) <= gunDistanceFromBossy)
            {
                lockingPosition = lockingPosition is null ? new GameObject("LockingPosition") : lockingPosition;
                Vector2 lockedPositionOfGun = bossY.gun.transform.position;
                lockingPosition.transform.position = lockedPositionOfGun;
                lockingPosition.transform.SetParent(bossY.transform);
                lockGunPosition = true; 
            }
        }
        else
        {
            bossY.gun.transform.position = lockingPosition.transform.position;
        }


        return Node.NodeStatus.RUNNING;
    }

    private void HandleCollision(bool activate)
    {
        
        Physics2D.IgnoreLayerCollision(bossY.gameObject.layer, LayerMask.NameToLayer("Ground"),activate);
       
    }
}

public class SendGunToPointAndRotateStrategy : MainStrategyForBossY, IStrategy
{
    Transform randomGunPoint;
    bool isPointSelected = false;
    float gunMoveSpeed = 7f;
    float gunDistanceFromPoint = 0.1f;
    float angle;

    float gunRotationSpeed = 200f;

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("SendGunToPointAndRotateStrategy");
        if (!isPointSelected)
        {
            bossY.levelController.SetPlatforms();
            randomGunPoint = bossY.gunPoints[Random.Range(0, bossY.gunPoints.Count)];
            angle = bossY.gun.transform.eulerAngles.z % 360;
            Debug.Log(angle);
            isPointSelected = true;
        }

        if (Vector2.Distance(bossY.gun.transform.position, randomGunPoint.transform.position) < 0.1f)
        {   
            if(angle < 180)
            {
                bossY.gun.transform.rotation = Quaternion.Euler(0, 0, angle);
                angle += Time.deltaTime * gunRotationSpeed;
                if(angle > 180)
                {
                    isPointSelected = false;
                    bossY.gun.transform.rotation = Quaternion.Euler(0, 0, 180);
                    return Node.NodeStatus.SUCCESS;
                }
 
            }
            if(angle > 180)
            {
                bossY.gun.transform.rotation = Quaternion.Euler(0, 0, angle);
                angle -= Time.deltaTime * gunRotationSpeed;
                if (angle < 180)
                {
                    isPointSelected = false;
                    bossY.gun.transform.rotation = Quaternion.Euler(0, 0, 180);
                    return Node.NodeStatus.SUCCESS;
                }

            }

            if(angle == 180)
            {
                isPointSelected= false;
                return Node.NodeStatus.SUCCESS; 
            }
            return Node.NodeStatus.RUNNING;
        }
        else
        {
            bossY.gun.transform.position =
                Vector2.MoveTowards(bossY.gun.transform.position, randomGunPoint.transform.position, Time.deltaTime * gunMoveSpeed);
            return Node.NodeStatus.RUNNING;
        }
    }

}

public class ChangeGunPositionAndShootStrategy : MainStrategyForBossY, IStrategy
{
    List<Transform> positionsToMoves => 
        bossY.gunPoints.Where(point => Vector2.Distance(point.transform.position, bossY.gun.transform.position) > 0.1f).ToList();
    Transform choosedPosition;
    float secondsToWaitShooting = 0.1f;
    float gunMoveSpeed = 24f;
    float distance = 0.1f;
    int random => Random.Range(0, positionsToMoves.Count);

    bool canMove = true;
    bool isInShoot = false;
    bool positionSetted = false;
    bool initShooting = true;
    bool canShoot = false;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("ChangeGunPositionAndShootStrategy");
        if (initShooting)
        {
            isInShoot = true;
            initShooting = false;
            canMove = false;
            bossY.gun.Shoot(Vector2.down,bossY.gun.transform.eulerAngles.z);
            bossY.StartCoroutine(WaitShooting());

        }

        if (playerController.punched)
        {
            ResetNode();
            bossY.levelController.SetPlatforms();
            return Node.NodeStatus.SUCCESS;
        }

        if (canMove)
        {   
            if (!positionSetted)
            {
                choosedPosition = positionsToMoves[random];
                positionSetted = true;
            }
            bossY.gun.transform.position = 
                Vector2.MoveTowards(bossY.gun.transform.position, choosedPosition.transform.position, Time.deltaTime * gunMoveSpeed);
            if(Vector2.Distance(bossY.gun.transform.position,choosedPosition.transform.position) < distance)
            {
                canMove = false;
                canShoot = true;
                positionSetted = false;
            }
            return Node.NodeStatus.RUNNING;
        }

        if (canShoot)
        {
            if (!isInShoot)
            {
                isInShoot = true;
                bossY.gun.Shoot(Vector2.down, bossY.gun.transform.eulerAngles.z);
                bossY.StartCoroutine(WaitShooting());
            }
        }

        return Node.NodeStatus.RUNNING;
    }

    private void ResetNode()
    {
        bossY.wideCam.Priority = 9;
        bossY.isSpecialTwoFinished = true;
        bossY.gun.isBelongToPlayer = true;
        canMove = true;
        isInShoot = false;
        positionSetted = false;
        playerController.punched = false;
        initShooting= true;
        canShoot = false;
    }

    private IEnumerator WaitShooting()
    {
        yield return new WaitForSeconds(secondsToWaitShooting);
        isInShoot = false;
        canMove = true;
        canShoot = false;
    }
}

