using Cinemachine;
using System;
using System.Collections;
using UnityEngine;


public abstract class MainStrategyForBossX : MainBossStrategy
{
    protected BossX bossX => blackBoard.GetValue("BossX", out BossX bossX) ? bossX : null;

}
public class CloseRangeAttackStrategyForBossX : MainStrategyForBossX, IStrategy
{
    public CloseRangeAttackStrategyForBossX(bool isBelong = false)
    {
        belongsSpecialAttackBranch = isBelong;
    }
    public Node.NodeStatus Evaluate()
    {

        Debug.Log("Close Range attack");

        if (!bossX.isInCloseRangeAttack)
        {
            bossX.isInCloseRangeAttack = true;

        }
        AnimatorStateInfo stateInfo = bossX.bossAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("CloseRangeAttack"))
        {

            if (stateInfo.normalizedTime >= 1f)
            {
                bossX.isInCloseRangeAttack = false;

                if (bossX.specialOneCoroutineBlocker && belongsSpecialAttackBranch)
                    bossX.specialOneCoroutineBlocker = false;

                bossX.probOfSpecialOneAttack = 0;
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

public class LongRangeAttackStrategyForBossX : MainStrategyForBossX, IStrategy
{
    public LongRangeAttackStrategyForBossX(bool isBelong)
    {
        belongsSpecialAttackBranch = isBelong;
    }
    public Node.NodeStatus Evaluate()
    {
        bossX.rb.velocity = new Vector2(0, bossX.rb.velocity.y);
        Debug.Log("Long Range attack");
        if (!bossX.isInLongRangeAttack)
        {
            bossX.isInLongRangeAttack = true;

        }


        AnimatorStateInfo stateInfo = bossX.bossAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("LongRangeAttack"))
        {
            if (stateInfo.normalizedTime >= 1)
            {
                if (bossX.specialOneCoroutineBlocker && belongsSpecialAttackBranch)
                    bossX.specialOneCoroutineBlocker = false;
                bossX.isInLongRangeAttack = false;
                bossX.probOfLongRangeAttack = 0;
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

public class JumpToAWayPointStrategy : MainStrategyForBossX, IStrategy
{
    public enum Method
    {
        closest,
        farest,
    }

    Method whichWayPoint;

    public JumpToAWayPointStrategy(Method whichWayPoint)
    {
        this.whichWayPoint = whichWayPoint;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("JumpToAWayPoint");
        if (!blockCoroutine)
        {
            isInProgress = true;
            GameManager.instance.StartCoroutine(DrawPathAndFollow());
            blockCoroutine = true;
        }

        if (isInProgress)
        {
            return Node.NodeStatus.RUNNING;
        }
        else
        {
            bossX.probOfSpecialOneAttack = 0;
            blockCoroutine = false;
            return Node.NodeStatus.SUCCESS;
        }

    }


    private IEnumerator DrawPathAndFollow()
    {
        Vector2 wayPoint = Vector2.zero;

        if (whichWayPoint == Method.farest)
        {
            wayPoint = Vector2.Distance(playerController.transform.position, bossX.wayPointLeft.position)
            >= Vector2.Distance(playerController.transform.position, bossX.wayPointRight.position)
            ? bossX.wayPointLeft.position : bossX.wayPointRight.position;
        }

        if(whichWayPoint == Method.closest)
        {
            wayPoint = Vector2.Distance(playerController.transform.position, bossX.wayPointLeft.position)
            <= Vector2.Distance(playerController.transform.position, bossX.wayPointRight.position)
            ? bossX.wayPointLeft.position : bossX.wayPointRight.position;
        }
        

        Vector2 referencePoint = bossX.transform.position;
        float x3 = wayPoint.x > bossX.transform.position.x ?
            referencePoint.x + Vector2.Distance(wayPoint, referencePoint) / 2f:
            referencePoint.x - Vector2.Distance(wayPoint, referencePoint) / 2f;

        float y3 = 4f;
        float time = 0.6f;
        float x1 = referencePoint.x;
        float x2 = wayPoint.x;

        if (wayPoint.x > bossX.transform.position.x)
        {
            for (float passedTime = 0; passedTime < 1f; passedTime += Time.deltaTime / time)
            {
                float x = Mathf.Lerp(referencePoint.x, wayPoint.x, passedTime);
                float y = CreateParabolaAndReturnValue(x, referencePoint, x1, x2, x3, y3);
                bossX.yVelocity = x < x3 ? 1 : -1;
                bossX.transform.position = new Vector2(x, y);

                yield return null;
            }
        }
        else
        {
            for (float passedTime = 1f; passedTime > 0f; passedTime -= Time.deltaTime / time)
            {
                float x = Mathf.Lerp(wayPoint.x, referencePoint.x, passedTime);
                float y = CreateParabolaAndReturnValue(x, referencePoint, x1, x2, x3, y3);
                bossX.yVelocity = x > x3 ? 1 : -1;
                bossX.transform.position = new Vector2(x, y);

                yield return null;
            }
        }
        isInProgress = false;

    }

    private float CreateParabolaAndReturnValue(float x, Vector2 referencePoint, float x1, float x2, float x3, float y3)
    {

        float A = y3 / ((x3 - x1) * (x3 - x2));

        float y = A * (x - x1) * (x - x2) + referencePoint.y;

        return y;
    }

}

public class DashAttackStrategy : MainStrategyForBossX, IStrategy
{
    float dashSpeed = 35f;
    bool isDashReady = false; 
    AnimationClip attackAfterDash;
    float distance => Vector2.Distance(bossX.transform.position, playerController.transform.position);

    public DashAttackStrategy(bool isBelong)
    {
        GetAnimationClip();
        AddAnimationEvent();
        belongsSpecialAttackBranch = isBelong;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DashAttackStrategy");
        if (!blockCoroutine)
        {
            GameManager.instance.StartCoroutine(ReadyAttack());
            blockCoroutine = true;
            bossX.isDashAttackInProgress = true;
           
        }
        if (isDashReady)
        {
            DashAndAttack();
        }

        if (bossX.isDashAttackInProgress)
        {
            return Node.NodeStatus.RUNNING;
        }
        else
        {
            if(bossX.specialOneCoroutineBlocker && belongsSpecialAttackBranch)
            {
                bossX.specialOneCoroutineBlocker = false;
                  
            }
            bossX.probOfSpecialOneAttack = 0;

            blockCoroutine = false;
            
            return Node.NodeStatus.SUCCESS;
        }
    }

    private IEnumerator ReadyAttack()
    {
        bossX.isStanceReady = true;
        yield return new WaitForSeconds(belongsSpecialAttackBranch ? 0.3f : 0.1f);
        isDashReady = true;
    }

    private void DashAndAttack()
    {
        if (distance < 4f)
        {
            bossX.rb.velocity = new Vector2(0, bossX.rb.velocity.y);
            bossX.bossAnim.SetTrigger("attackAfterDash");
            isDashReady = false;
            bossX.isStanceReady = false;
            return;
        }
        bossX.rb.velocity = new Vector2(dashSpeed * bossX.direction.x, bossX.rb.velocity.y);

    }

    private void GetAnimationClip()
    {
        RuntimeAnimatorController controller = bossX.bossAnim.runtimeAnimatorController;
        
        foreach(var clip in controller.animationClips)
        {
            if (clip.name == "attack_after_dash")
            {
                attackAfterDash = clip;
            }
        }

    }
    private void AddAnimationEvent()
    {
        AnimationEvent animationEvent = new AnimationEvent();
        animationEvent.time = attackAfterDash.length;
        animationEvent.functionName = "EndProgress";
        attackAfterDash.AddEvent(animationEvent);
    }

}

class JumpAboveStrategy : MainStrategyForBossX, IStrategy
{
    float speed = 7f;
    public Node.NodeStatus Evaluate()
    {
        bossX.rb.velocity = new Vector2(0, bossX.rb.velocity.y);
        Debug.Log("JumpAbove");
        if (!blockCoroutine)
        {   
            bossX.StartCoroutine(JumpOutOfBoundaries());
            isInProgress = true;
            blockCoroutine = true;
            bossX.isSpecialTwoReady = true;   
        }

        if (isInProgress)
        {
            return Node.NodeStatus.RUNNING;
        }
        else
        {
            blockCoroutine = false;
            bossX.inSpecialTwo = true;
            return Node.NodeStatus.SUCCESS;
        }
        
    }

    public IEnumerator JumpOutOfBoundaries()
    {
        
        bossX.rb.gravityScale = 0;
        bossX.yVelocity = 1;

        for (float point = 0; point < 1f; point += Time.deltaTime / speed)
        {
            if (bossX.isUpThere)
            {
                bossX.yVelocity = 0;
                GameManager.instance.StartCoroutine(WaitAbove());
                break;
            }
            float y = Mathf.Lerp(bossX.transform.position.y, bossX.upPoint.transform.position.y, point);
            bossX.transform.position = new Vector2(bossX.transform.position.x, y);

            yield return null;
        }

    }

    public IEnumerator WaitAbove()
    {
        yield return new WaitForSeconds(1.5f);
        isInProgress = false;
    }
}


class LandAndInflictDamageStrategy : MainStrategyForBossX, IStrategy
{
    float speed = 0.4f;
    float duration;
    CinemachineBasicMultiChannelPerlin channel => blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null; 
    BoxCollider2D particleCollider => bossX.groundImpactParticle.GetComponent<BoxCollider2D>();

    Vector2 closestWayPointPosToPlayer =>
        Vector2.Distance(bossX.wayPointLeft.transform.position, playerController.transform.position) <
        Vector2.Distance(bossX.wayPointRight.transform.position, playerController.transform.position) ? bossX.wayPointLeft.position : bossX.wayPointRight.position;

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("LandInflict");
        if (!blockCoroutine)
        {
            GameManager.instance.StartCoroutine(StartLanding());
            blockCoroutine = true;
            isInProgress = true;
        }

        if (isInProgress)
        {
            
            return Node.NodeStatus.RUNNING;
        }
        else
        {
            isInProgress = false;
            blockCoroutine = false;
            return Node.NodeStatus.SUCCESS;
        }
    }

    private IEnumerator StartLanding()
    {
        float capturedXPosOfPlayer = playerController.offset.transform.position.x > bossX.wayPointLeft.position.x &&
            playerController.offset.transform.position.x < bossX.wayPointRight.position.x ? playerController.offset.transform.position.x :
            closestWayPointPosToPlayer.x;
        float groundYPosOfPlayer = bossX.downPoint.transform.position.y;
        float startingPoint = bossX.transform.position.y;
        bossX.transform.position = new Vector2(capturedXPosOfPlayer, bossX.transform.position.y);
        bossX.rb.gravityScale = 0;
        bossX.yVelocity = -1;

        for (float point = 1f; point > 0f; point -= Time.deltaTime / speed)
        {
            if (!bossX.isJumped)
            {
                bossX.yVelocity = 0;
                bossX.onLand = true;
                duration = bossX.groundImpactParticle.main.duration;
                GameManager.instance.StartCoroutine(WaitOnTheGround(duration));
                bossX.rb.gravityScale = bossX.defaultGravity;
                break;
            }
            float y = Mathf.Lerp(groundYPosOfPlayer, startingPoint, point);

            bossX.transform.position = new Vector2(bossX.transform.position.x, y);

            yield return null;
        }
    }

    private IEnumerator WaitOnTheGround(float duration)
    {
        bossX.groundImpactParticle.transform.position = bossX.offsetWayPoint.position;
        bossX.groundImpactParticle.Play();
        particleCollider.enabled = true;
        channel.m_AmplitudeGain = 2.5f;
        yield return new WaitForSeconds(duration);
        channel.m_AmplitudeGain = 0f;
        particleCollider.enabled = false;
        bossX.onLand = false;
        isInProgress = false;
        bossX.inSpecialTwo = false;
    }
}

public class DieStrategy : MainStrategyForBossX, IStrategy
{
    public Node.NodeStatus Evaluate()
    {
        if (!bossX.inDeathProgressed)
        {
            bossX.inDeathToSayIsReady = true;
            bossX.inDeathProgressed = true;
        }
        bossX.canAvatarDie = true;
        return Node.NodeStatus.SUCCESS;
    }
}

public class DoNothingStrategy : MainStrategyForBossX, IStrategy
{
    //say something after death of player
    public Node.NodeStatus Evaluate()
    {
        if (!bossX.inCharacterDeathProgressed)
        {
            bossX.inCharacterDeathToSayIsReady = true;
            bossX.inCharacterDeathProgressed = true;
        }
        
        return Node.NodeStatus.SUCCESS;
    }
}


