using UnityEngine;
using AdvancedStateHandling;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{

    public Animator playerAnimController;
    public float jumpForce;
    public Rigidbody2D rb;
    public SpriteRenderer playerRenderer;
    public float charSpeed;
    public float xAxis;
    public float doubleJumpForce;
    [SerializeField] private Collider2D groundCheck;
    public Transform offset;

    float originalDrag;

    public ParticleSystem dashParticles;

    public float timeToPassInSlide;
    public float timeToPass;
    public float slideForce;

    public bool isSliding;
    public bool getUpAnimController;
    public bool jump;
    public bool controlDoubleJumpAnim;
    public bool doubleJumped;
    public bool jumpedInJumpState;
    public bool jumpedInFallState;
    public bool canRoll;
    public bool isDead;
    public bool isFacingRight = true;
    public bool isInPerfectDash;
    public bool isInDash;
    public bool isDamaged;
    public bool dashInCoolDown;

    [Header("Collision Detection Segment")]
    public bool freeToGetUp;
    public LayerMask ground;
    public bool isJumped;
    [SerializeField] private float distanceFromGround;
    [SerializeField] private float distanceToAbove;

    [Header("LedgeDetection")]
    public bool ledgeDetected;
    public Vector2 offSet1;
    public Vector2 offSet2;
    public Vector2 startOfLedgGrabPos;
    public Vector2 endOfLedgeGrabPos;
    public bool canGrabLedge = true;
    public Transform playerFrame;

    [Header("Health")]
    public PlayerHealthBar playerHealthBar;
    public float maxHealth = 100;
    public float currentHealth;

    public AdvancedStateMachine stateMachine;
    BlackBoard blackBoard;
    void Start()
    {
        DontDestroyOnLoad(this);
        currentHealth = maxHealth;

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            playerHealthBar = GameObject.Find("Canvas").transform.Find("Player HealthBar").GetComponent<PlayerHealthBar>();
            playerHealthBar.SetMaxHealth(maxHealth);
        }

        blackBoard = GameManager.Instance.blackBoard;

        rb = GetComponent<Rigidbody2D>();
        originalDrag = rb.drag;
        playerRenderer = GetComponent<SpriteRenderer>();

        stateMachine = new AdvancedStateMachine();
        
        blackBoard.SetValue("PlayerController", this);

        var jumpState = new JumpState(this);
        var moveState = new MoveState(this);
        var slideState = new SlideState(this);
        var fallState = new FallState(this);
        var ledgeGrabState = new LedgeGrabState(this);
        var doubleJumpState = new DoubleJumpState(this);
        var afterDoubleJumpFallState = new AfterDoubleJumpFallState(this);
        var rollState = new RollState(this);
        var dashState = new DashState(this);
        var damageState = new DamageState(this);
        var deathState = new DeathState(this);

        At(afterDoubleJumpFallState, rollState, new FuncPredicate(() => canRoll && rb.velocity.y == 0));
        At(fallState, rollState, new FuncPredicate(() => canRoll && rb.velocity.y == 0));
        At(moveState, jumpState, new FuncPredicate(() => jump));
        At(jumpState,fallState, new FuncPredicate(() => isJumped && rb.velocity.y < 0));
        At(fallState, moveState, new FuncPredicate(() => !isJumped && rb.velocity.y == 0));
        At(moveState, slideState, new FuncPredicate(() => isSliding && SceneManager.GetActiveScene().buildIndex == 0));
        At(slideState, moveState, new FuncPredicate(() => !isSliding && freeToGetUp));
        At(slideState, fallState, new FuncPredicate(() => isJumped));
        At(moveState, ledgeGrabState, new FuncPredicate(() => ledgeDetected));
        At(jumpState, ledgeGrabState, new FuncPredicate(() => ledgeDetected));
        At(fallState, ledgeGrabState, new FuncPredicate(() => ledgeDetected));
        At(ledgeGrabState, moveState, new FuncPredicate(() => !ledgeDetected));
        At(jumpState, doubleJumpState, new FuncPredicate(() => doubleJumped));
        At(fallState, doubleJumpState, new FuncPredicate(() => doubleJumped));
        At(doubleJumpState, ledgeGrabState, new FuncPredicate(() => ledgeDetected));
        At(doubleJumpState, afterDoubleJumpFallState, new FuncPredicate(() => rb.velocity.y < 0));
        At(afterDoubleJumpFallState, moveState, new FuncPredicate(() => !isJumped && rb.velocity.y == 0));
        At(afterDoubleJumpFallState, ledgeGrabState, new FuncPredicate(() => ledgeDetected));
        At(rollState, moveState, new FuncPredicate(() => !canRoll));
        At(moveState, dashState, new FuncPredicate(() => isInDash));
        At(jumpState, dashState, new FuncPredicate(() => isInDash));
        At(fallState, dashState, new FuncPredicate(() => isInDash));
        At(afterDoubleJumpFallState, dashState, new FuncPredicate(() => isInDash));
        At(doubleJumpState, dashState, new FuncPredicate(() => isInDash && !dashInCoolDown));
        At(dashState, moveState, new FuncPredicate(() => !isJumped && !isInDash));
        At(dashState, afterDoubleJumpFallState, new FuncPredicate(() => isJumped && rb.velocity.y < 0 && !isInDash));

        Any(damageState, new FuncPredicate(() => isDamaged && !isDead));
        At(damageState, afterDoubleJumpFallState, new FuncPredicate(() => !isDamaged && rb.velocity.y < 0));
        At(damageState, moveState, new FuncPredicate(() => !isDamaged && rb.velocity.y == 0));

        Any(deathState, new FuncPredicate(() => isDead));
        stateMachine.currentState = moveState;
    }

    private void At(IState from, IState to, IPredicate condition)
    {
        stateMachine.AddTransition(from, to, condition);
    }

    private void Any(IState to, IPredicate condition)
    {
        stateMachine.AddTransitionFromAnytate(to, condition);
    }

    void Update()
    {
        
        CheckCollision();
        SetRotation();
        AnimationController();
        stateMachine.Update();
    }

    public void CheckHighness()
    {
        if(rb.velocity.y < -10)
        {
            canRoll = true;
        }
        
    }

    private void AtEndOfLedgeClimb()
    {
        
        transform.position = endOfLedgeGrabPos;
        
        rb.gravityScale = 2;
  
    }

    private void AtEndOfRoll() => canRoll = false;

   
    public void StartSliding()
    {  
        isSliding = true;
    }

    private void HandleGetUpSit() => getUpAnimController = false;
    
    private void ResDoubleJumpSit() => controlDoubleJumpAnim = false;

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x,transform.position.y + distanceToAbove));
    }

    private void CheckCollision()
    {
      
        freeToGetUp = !Physics2D.Raycast(transform.position, Vector2.up, distanceToAbove, ground);
        isJumped = !Physics2D.BoxCast(groundCheck.transform.position,groundCheck.bounds.size,0,Vector2.zero,0,ground);
    }
    private void AnimationController()
    {
        playerAnimController.SetFloat("xDirection", xAxis);
        playerAnimController.SetFloat("yDirection", rb.velocity.normalized.y);

        playerAnimController.SetBool("isJumped", isJumped);
        playerAnimController.SetBool("doubleJumped", controlDoubleJumpAnim);
        playerAnimController.SetBool("isSliding", isSliding);
        playerAnimController.SetBool("getUp", getUpAnimController);
        playerAnimController.SetBool("edgeDetected", ledgeDetected);
        playerAnimController.SetBool("canRoll",canRoll);
        playerAnimController.SetBool("isInDash", isInDash);
        playerAnimController.SetBool("isDead", isDead);
        
    }

    private void SetRotation()
    {
        if (xAxis == 1 && !isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingRight = true;
        }

        if (xAxis == -1 && isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    public void HandleMovement()
    {
        xAxis = Input.GetAxisRaw("Horizontal");   
    }

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if(playerHealthBar == null)
        {
            return;
        }
        playerHealthBar.SetCurrentHealth(currentHealth);
        if(currentHealth <= 0)
        {
            isDead = true;
        }
        
    }

   
}
