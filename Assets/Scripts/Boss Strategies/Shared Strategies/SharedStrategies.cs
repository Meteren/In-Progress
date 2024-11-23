using System;
using UnityEngine;

public class Condition: IStrategy
{
    Func<bool> condition { get; set; }

    public Condition(Func<bool> condition)
    {
        this.condition = condition;
    }
    public Node.NodeStatus Evaluate()
    {

        Debug.Log("Condition" + condition());
        if (condition())
        {
            return Node.NodeStatus.SUCCESS;
        }
        else
        {
            Debug.Log("Failure");
            return Node.NodeStatus.FAILURE;
        }
    }

}

public class StayStillStrategy : IStrategy
{
    Boss boss;
    public StayStillStrategy(Boss boss)
    {
        this.boss = boss;
    }
    public Node.NodeStatus Evaluate()
    {

        Debug.Log("StayStill");
        boss.rb.velocity = new Vector3(0, boss.rb.velocity.y);
        return Node.NodeStatus.SUCCESS;
    }

}

public class ChasePlayerStrategy : IStrategy
{
    int chaseSpeed;
    Boss boss;

    public ChasePlayerStrategy(Boss boss,int speed)
    {
        this.boss = boss;
        this.chaseSpeed = speed;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Chase Player");
        boss.rb.velocity = new Vector2(boss.direction.x * chaseSpeed, boss.rb.velocity.y);
        return Node.NodeStatus.SUCCESS;

    }

}


