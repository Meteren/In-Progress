using AdvancedStateHandling;
using UnityEngine;

public class SummonedSpirit : MonoBehaviour
{
    PlayerController controller =>
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    BossY bossY => GameManager.instance.blackBoard.GetValue("BossY", out BossY _bossY) ? _bossY : null;

    public Vector2 playerDirection => (controller.transform.position - transform.position).normalized;
    Vector2 currentDirection;

    public Collider2D generationFrame;
    public Rigidbody2D rb;

    public bool isOffenseSpirit;
    bool isFacingLeft = true;
    public bool idle;
    public bool canMoveTo = false;
    public bool activateOrbitalMove;
    public bool activateChase;
    public bool departed = false;
    public bool selectRandomPos = false;

    public Animator spiritAnim;
    public MoveToPointState moveToPointState;
    public Vector2 defenseSpiritMoveToPosition;

    [HideInInspector]
    public Transform centerPoint;
    public bool isAttached;
    public Vector2 spiritSpawnPoint;

    public AdvancedStateMachine spiritStateMachine = new AdvancedStateMachine();
   
    void Update()
    {
        if(spiritStateMachine.currentState != null)
            spiritStateMachine.Update();
        SetDirection(currentDirection);
        SetRotation();
        AnimationController();
    }

    public void Init(Transform centerPoint, float angle, float radius,bool isOffenseSpirit,bool isAttached, Collider2D generationFrame)
    {
        this.centerPoint = centerPoint;
        this.isOffenseSpirit = isOffenseSpirit;
        this.isAttached = isAttached;
        this.generationFrame = generationFrame;
       
        if (isAttached)
        {
            SetPosition(angle,radius);
            
        }
        else
        {
            SetPosition();
        }

        var spawnState = new SpawnState(this, spiritSpawnPoint,angle);
        var orbitalMoveState = new OrbitalMoveState(this, angle, 
            isOffenseSpirit ? centerPoint.GetComponent<WayPoint>().radius + 2 : centerPoint.GetComponent<WayPoint>().radius);
        var chaseState = new ChaseState(this);
        moveToPointState = new MoveToPointState(this);
        Add(spawnState, orbitalMoveState, new FuncPredicate(() => activateOrbitalMove));
        Add(spawnState, moveToPointState, new FuncPredicate(() => canMoveTo));
        Add(orbitalMoveState, moveToPointState, new FuncPredicate(() => canMoveTo));
        Add(moveToPointState, orbitalMoveState, new FuncPredicate(() => activateOrbitalMove));
        Add(orbitalMoveState, chaseState, new FuncPredicate(() => activateChase));
        Add(moveToPointState, chaseState, new FuncPredicate(() => activateChase));

        spiritStateMachine.currentState = spawnState;   
        spawnState.OnStart();
    }

    private void Add(IState from, IState to, IPredicate condition)
    {
        spiritStateMachine.AddTransition(from, to, condition);
    }

    private void AnimationController()
    {
        spiritAnim.SetBool("idle",idle);
    }

    private void SetPosition(float angle,float radius)
    {
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        spiritSpawnPoint = new Vector2(centerPoint.transform.position.x + x, centerPoint.transform.position.y + y);

    }

    private void SetPosition()
    {
        spiritSpawnPoint = centerPoint.GetComponentInParent<Transform>().position;    
    }

    protected void SetRotation()
    {
        if (currentDirection.x < 0 && !isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingLeft = true;
        }

        if (currentDirection.x > 0 && isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingLeft = false;
        }
    }

    public void SetDirection(Vector2 direction)
    {
        if (isAttached)
            this.currentDirection = bossY.direction;
        else
            this.currentDirection = playerDirection;
    }

    public Vector2 GenerateRandomPosition()
    {
        float x = Random.Range(generationFrame.bounds.min.x, generationFrame.bounds.max.x);
        float y = Random.Range(generationFrame.bounds.center.y - (generationFrame.bounds.size.y / 2),
            generationFrame.bounds.center.y + (generationFrame.bounds.size.y / 2));

        return new Vector2(x, y);
    }

    private float InflictDamage()
    {
        return 3f;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (!controller.isInDash && !controller.isDead )
            {
                controller.isDamaged = true;
                controller.damageDirection = currentDirection;
                controller.OnDamage(InflictDamage());

                Destroy(gameObject);
            }
            
        }
    }


}
