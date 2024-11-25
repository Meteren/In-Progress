using Unity.VisualScripting;
using UnityEngine;

public class SpawnState : BaseSummonedSpiritState
{
    Vector2 spawnPoint;
    float angle;
    public SpawnState(SummonedSpirit spirit, Vector2 spawnPoint, float angle) : base(spirit)
    {
        this.spawnPoint = spawnPoint;
        this.angle = angle;
    }

    public override void OnStart()
    {
        base.OnStart();
        Debug.Log("Spawn Point" + spawnPoint);
        spirit.transform.position = spawnPoint;
       
    }
    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        AnimatorStateInfo currentStateInfo = spirit.spiritAnim.GetCurrentAnimatorStateInfo(0);

        if (currentStateInfo.IsName("Appear"))
        {
            if(currentStateInfo.normalizedTime >= 1)
            {
                if (spirit.isOffenseSpirit)
                {
                    
                    float x = Mathf.Cos(angle) * (spirit.centerPoint.GetComponent<WayPoint>().radius + 2f);
                    float y = Mathf.Sin(angle) * (spirit.centerPoint.GetComponent<WayPoint>().radius + 2f);
                    Vector2 moveTo = 
                        new Vector2(spirit.centerPoint.transform.position.x + x, spirit.centerPoint.transform.position.y + y);
                    spirit.moveToPointState.PositionToMove = moveTo;
                    spirit.canMoveTo = true;
                    return;

                }
                    
                if (spirit.isAttached)
                {
                    spirit.activateOrbitalMove = true;
                    spirit.idle = true;
                }
                else
                {
                    spirit.activateChase = true;
                }

                
            }
        }

    }

}

public class OrbitalMoveState : BaseSummonedSpiritState
{
    float angle;
    float radius;
    float baseRadius;
    float moveSpeed = 2f;
    float radiusIncreaseSpeed = 1f;
    float counter = 3f;
    float radiusIncreaseAmount = 4f;

    bool setRadius = false;
    bool radiusSettedWider = false;

    public OrbitalMoveState(SummonedSpirit spirit,float angle,float radius) : base(spirit)
    {
        this.radius = radius;
        this.angle = angle;
        this.baseRadius = this.radius;   
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();

        if (counter <= 0)
        {
            setRadius = true;
        }
        else
        {
            counter -= Time.deltaTime;
        }
                   
        Move();
        
    }

    private void Move()
    {
        
        float xPos = Mathf.Cos(angle) * radius;
        float yPos = Mathf.Sin(angle) * radius;
        
        /*if (setRadius)
        {
            if (!radiusSettedWider)
            {
                radius += Time.deltaTime * radiusIncreaseSpeed;
                if (radius >= baseRadius + radiusIncreaseAmount)
                {
                    setRadius = false;
                    radiusSettedWider= true;
                    counter = 3f;
                }
            }
            else
            {
                radius -= Time.deltaTime * radiusIncreaseSpeed;
                if (radius <= baseRadius)
                {
                    setRadius = false;
                    radiusSettedWider = false;
                    counter = 3f;
                    radius = baseRadius;
                }
            }

            
        }*/

        spirit.transform.position = new Vector2(spirit.centerPoint.transform.position.x + xPos,spirit.centerPoint.transform.position.y + yPos);

        angle += Time.deltaTime * moveSpeed;
    }
}

public class MoveToPointState : BaseSummonedSpiritState
{
    private Vector2 positionToMove;
    float moveSpeed = 6f;
    float distance = 0.1f;

    public Vector2 PositionToMove
    {
        get
        {
            return positionToMove;
        }
        set
        {
            positionToMove = value;
        }
    }
    public MoveToPointState(SummonedSpirit spirit) : base(spirit)
    {
    }
    public override void OnStart()
    {
        base.OnStart();
        
    }
    public override void OnExit()
    {
        base.OnExit();
        spirit.canMoveTo = false;
    }
    public override void Update()
    {
        base.Update();
        spirit.transform.position = Vector2.MoveTowards(spirit.transform.position, positionToMove, Time.deltaTime * moveSpeed);

        if(Vector2.Distance(spirit.transform.position, positionToMove) <= distance)
        {
            if (spirit.isOffenseSpirit)
            {
                spirit.activateOrbitalMove = true;
            }

            if (!spirit.isOffenseSpirit)
            {
                spirit.activateChase = true;
            }
        }
       
    }
}
