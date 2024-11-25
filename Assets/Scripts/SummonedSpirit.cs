using AdvancedStateHandling;
using UnityEngine;

public class SummonedSpirit : MonoBehaviour
{

    PlayerController controller =>
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    BossY bossY => GameManager.instance.blackBoard.GetValue("BossY", out BossY _bossY) ? _bossY : null;

    Vector2 playerDirection => (controller.transform.position - transform.position).normalized;
    Vector2 currentDirection;

    public bool isOffenseSpirit;
    bool isFacingLeft = true;
    public bool idle = false;
    public bool canMoveTo = false;

    public Animator spiritAnim;
    public bool activateOrbitalMove;
    public bool activateChase;
    public MoveToPointState moveToPointState;
    public Vector2 defenseSpiritMoveToPosition;

    [HideInInspector]
    public Transform centerPoint;
    public bool isAttached = true;
    public Vector2 spiritSpawnPoint;

    AdvancedStateMachine spiritStateMachine = new AdvancedStateMachine();
   
    void Update()
    {
        if(spiritStateMachine.currentState != null)
            spiritStateMachine.Update();
        SetDirection(currentDirection);
        SetRotation();
    }

    public void Init(Transform centerPoint, float angle, float radius,bool isOffenseSpirit)
    {
        this.centerPoint = centerPoint;
        this.isOffenseSpirit = isOffenseSpirit;
       
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
        moveToPointState = new MoveToPointState(this);
        Add(spawnState, orbitalMoveState, new FuncPredicate(() => activateOrbitalMove));
        Add(spawnState, moveToPointState, new FuncPredicate(() => canMoveTo));
        Add(orbitalMoveState, moveToPointState, new FuncPredicate(() => canMoveTo));
        Add(moveToPointState, orbitalMoveState, new FuncPredicate(() => activateOrbitalMove));

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

}
