using AdvancedStateHandling;
using Cinemachine;
using System.Collections;
using UnityEngine;

public class BossX : MonoBehaviour
{
    [SerializeField] private DialogueContainer firstToSay;
    [SerializeField] private DialogueContainer secondToSay;
    [SerializeField] private DialogueContainer inAttackToSay;
    [SerializeField] private GameObject panel;
    
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask bossXLayer;
    public SpriteRenderer bossXRenderer;

    public Animator bossXAnim;

    public Vector2 direction;

    public float xVelocity;
    public float yVelocity;
    public float duration;

    [Header("Conditions")]
    public bool firstIsReady;
    public bool secondIsReady;
    public bool isInAttackReady;
    public bool isFacingLeft = true;
    public bool isInCloseRangeAttack = false;
    public bool isInLongRangeAttack = false;
    public bool isJumped;
    public bool canAttack;
    public bool isDashReady = false;
    public bool isStanceReady = false;
    public bool isInProgress = false;
    public bool isDashAttackInProgress = false;
    public bool isUpThere = false;
    public bool onLand = false;
    public bool isDead = false;
    public bool canAvatarDie = false;

    [Header("Special Attack Conditions")]
    public bool specialOneCoroutineBlocker = false;
    public bool specialTwoCoroutineBlocker = false;
    public bool isSpecialOneReady = false;
    public bool isSpecialTwoReady = false;

    [HideInInspector]public float probOfLongRangeAttack;
    [HideInInspector]public float probOfSpecialOneAttack;
    [HideInInspector]public float probOfSpecialTwoAttack;

    [Header("Rb")]
    public Rigidbody2D rb;

    AdvancedStateMachine dialogueStateMachine;

    BehaviourTree bossXBehaviourTree;

    BlackBoard blackBoard;

    [SerializeField] private float distanceToGround;
    [SerializeField] private float distanceToRight;
    [SerializeField] private float distanceToLeft;

    [Header("WayPoints")]
    public Transform wayPointRight;
    public Transform wayPointLeft;
    public Transform upPoint;
    public Transform downPoint;
    public Transform offsetWayPoint;

    float distanceToPlayer => Vector2.Distance(transform.position, playerController.transform.position);

    float bossXDistanceToLeftWayPoint => Vector2.Distance(transform.position, wayPointLeft.transform.position);
    float bossXDistanceToRightWayPoint => Vector2.Distance(transform.position, wayPointRight.transform.position);


    float centerPoint => (Vector2.Distance(wayPointLeft.transform.position, wayPointRight.transform.position)) / 2;

    public float defaultGravity;
    public ParticleSystem groundImpactParticle;
    
    [Header("Health")] 
    public BossXHealthBar bossXHealthBar;
    public float currentHealth;
    public float maxHealth = 100;

    private void Start()
    {
        
        currentHealth = maxHealth;
        //bossXHealthBar.SetMaxHealth(maxHealth);
        
        defaultGravity = rb.gravityScale;
        IgnoreCollision();

        blackBoard = GameManager.instance.blackBoard;

        blackBoard.SetValue("BossX", this);

        bossXBehaviourTree = new BehaviourTree("BossXBehaviourTree");
        dialogueStateMachine = new AdvancedStateMachine();

        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");
        SortedSelectorNode selectJumpType = new SortedSelectorNode("SelectJumpType");
        SequenceNode farestWayPointJumpSequence = new SequenceNode("FarestWayPointJumpSequence",10);
        SequenceNode closestWayPointJumpSequence = new SequenceNode("ClosestWayPointJumpSequence",20);

        Leaf farestWayPointJumpCondition = new Leaf("FarestWayPointJumpCondition", new Condition(() => IsTooCloseToAWayPoint()));
        Leaf closestWayPointJumpCondition = new Leaf("ClosestWayPointJumpCondition", new Condition(() => !IsTooCloseToAWayPoint()));
        Leaf farestWayPointJumpStrategy = new Leaf("FarestWayPointJumpStrategy",new JumpToAWayPointStrategy(JumpToAWayPointStrategy.Method.farest));
        Leaf closestWayPointJumpStrategy = new Leaf("ClosestWayPointJumpStrategy", new JumpToAWayPointStrategy(JumpToAWayPointStrategy.Method.closest));

        SequenceNode dieSequence = new SequenceNode("DieSequence",2);
        Leaf dieCondition = new Leaf("DieCondition", new Condition(() => isDead));
        Leaf dieStrategy = new Leaf("DieStrategy", new DieStrategy());

        SequenceNode doNothingSequence = new SequenceNode("DoNothingSequence", 1);
        Leaf doNothingCondition = new Leaf("DoNothingCondition", new Condition(() => playerController.isDead));
        Leaf doNothingStrategy = new Leaf("DoNothingStrategy", new DoNothingStrategy());

        SortedSelectorNode selectSpecialAttack = new SortedSelectorNode("SelectSpecialAttack",5);
        SequenceNode specialAttackOneSequence = new SequenceNode("SpecialAttackOneSequence", 10);
        SequenceNode specialAttackTwoSequence = new SequenceNode("SpecialAttackTwoSequence",5);

        Leaf specialAttackTwoReady = new Leaf("SpecialAttackTwoReady", new Condition(() => {
             if (!specialTwoCoroutineBlocker)
             {
                StartCoroutine(GenerateNumberForSpecialAttackTwo());
             }
             return (probOfSpecialTwoAttack > 50 && probOfSpecialTwoAttack < 100) && !firstIsReady;
        }));
        SequenceNode startSpecialAttackTwoSequence = new SequenceNode("StartSpecialAttackTwoSequence");

        Leaf jumpAboveStrategy = new Leaf("JumpAboveStrategy", new JumpAboveStrategy());
        Leaf landAndInflictDamageStrategy = new Leaf("LandAndInflictDamage", new LandAndInflictDamageStrategy());


        SequenceNode normalCloseRangeAttackSequence = new SequenceNode("CloseRangeAttackSequence", 20);
        SequenceNode specialCloseRangeAttackSequence = new SequenceNode("SpecialCloseRangeAttackSequence", 20);
        SequenceNode randomLongRangeAttackSequence = new SequenceNode("RandomLongRangeAttackSequence", 10);
        SequenceNode specialLongRangeAttackSequence = new SequenceNode("SpecialLongRangeAttackSequence",10);
        SortedSelectorNode selectNormalAttacks = new SortedSelectorNode("SelectNormalAttacks", 10);
        SequenceNode chaseSequence = new SequenceNode("ChaseSequence", 20);

        Leaf stayStillStrategy = new Leaf("StayStill", new StayStillStrategy(), 30);
        Leaf chaseCondition = new Leaf("ChaseCondition", new Condition(() => !firstIsReady));
        Leaf chasePlayerStrategy = new Leaf("ChasePlayerStrategy", new ChasePlayerStrategy());
        Leaf closeRangeAttackCondition = new Leaf("AttackOneCondition", new Condition(() =>
                    distanceToPlayer < 2.4f || isInCloseRangeAttack));
        Leaf normalCloseRangeAttackStrategy = new Leaf("NormalCloseRangeAttackStrategy", new CloseRangeAttackStrategy(false));
        Leaf specialCloseRangeAttackStrategy = new Leaf("SpecialCloseRangeAttackStrategy", new CloseRangeAttackStrategy(true));
        
        Leaf randomLongRangeAttackCondition = new Leaf("LongeRangeAttackCondition", new Condition(() =>
        {
            if(!isInProgress)
                StartCoroutine(GenerateNumberForRandomAttack());
            return ((distanceToPlayer > 2.4f && distanceToPlayer < 5.6f) && (probOfLongRangeAttack > 60 && probOfLongRangeAttack < 100) || isInLongRangeAttack);
        }));

        Leaf longRangeAttackCondition = new Leaf("LongeRangeAttackCondition", new Condition(() => (distanceToPlayer > 2.4f && distanceToPlayer < 5.6f) || isInLongRangeAttack));

        Leaf normalLongRangeAttackStrategy = new Leaf("NormalLongRangeAttackStrategy", new LongRangAttackStrategy(false));
        Leaf specialLongRangeAttackStrategy = new Leaf("SpecialLongRangeAttackStrategy", new LongRangAttackStrategy(true));

        Leaf specialDashAttackStrategy = new Leaf("AttackAfterDashStrategy", new DashAttackStrategy(true));

        Leaf dashAttackCondition = new Leaf("DashAttackCondition", new Condition(() => distanceToPlayer > 5.6f));

        SequenceNode specialDashAttackSequence = new SequenceNode("SpecialDashAttackSequence",5);
  
        Leaf specialAttackOneReady = new Leaf("SpecialAttackOneReady", new Condition(() =>
        {
            if (!specialOneCoroutineBlocker)
            {
                StartCoroutine(GenerateNumberForSpecialAttackOne());
            }            
            return (probOfSpecialOneAttack > 30 && probOfSpecialOneAttack < 100) && !firstIsReady;  
        }));
        
        SequenceNode startSpecialOneSequence = new SequenceNode("StartSpecialOneSequence");
       
        SortedSelectorNode selectAttackAfterJump = new SortedSelectorNode("SelectAttackAfterJump");
       
        //Special attack one sequence
        specialAttackOneSequence.AddChild(specialAttackOneReady);
        specialAttackOneSequence.AddChild(startSpecialOneSequence);

        //start special one sequence
        startSpecialOneSequence.AddChild(selectJumpType);
        startSpecialOneSequence.AddChild(selectAttackAfterJump);

        //add jump types
        selectJumpType.AddChild(farestWayPointJumpSequence);
        selectJumpType.AddChild(closestWayPointJumpSequence);

        //ready jump sequences
        farestWayPointJumpSequence.AddChild(farestWayPointJumpCondition);
        farestWayPointJumpSequence.AddChild(farestWayPointJumpStrategy);
        closestWayPointJumpSequence.AddChild(closestWayPointJumpCondition);
        closestWayPointJumpSequence.AddChild(closestWayPointJumpStrategy);

        //attack style selector after jump
        selectAttackAfterJump.AddChild(specialDashAttackSequence);
        selectAttackAfterJump.AddChild(specialLongRangeAttackSequence);
        selectAttackAfterJump.AddChild(specialCloseRangeAttackSequence);

        //special close range attack sequence
        specialCloseRangeAttackSequence.AddChild(closeRangeAttackCondition);
        specialCloseRangeAttackSequence.AddChild(specialCloseRangeAttackStrategy);

        //normal close range attack sequence
        normalCloseRangeAttackSequence.AddChild(closeRangeAttackCondition);
        normalCloseRangeAttackSequence.AddChild(normalCloseRangeAttackStrategy);

        //dash attack sequence
        specialDashAttackSequence.AddChild(dashAttackCondition);
        specialDashAttackSequence.AddChild(specialDashAttackStrategy);

        //random long range attack sequence
        randomLongRangeAttackSequence.AddChild(randomLongRangeAttackCondition);
        randomLongRangeAttackSequence.AddChild(normalLongRangeAttackStrategy);

        //special long range attack Sequence
        specialLongRangeAttackSequence.AddChild(longRangeAttackCondition);
        specialLongRangeAttackSequence.AddChild(specialLongRangeAttackStrategy);

        //Chase Player Sequence
        chaseSequence.AddChild(chaseCondition);
        chaseSequence.AddChild(chasePlayerStrategy);

        //Choose attack between long range and close range
        selectNormalAttacks.AddChild(normalCloseRangeAttackSequence);
        selectNormalAttacks.AddChild(randomLongRangeAttackSequence);

        //Start special attack two sequence
        startSpecialAttackTwoSequence.AddChild(jumpAboveStrategy);
        startSpecialAttackTwoSequence.AddChild(landAndInflictDamageStrategy);

        //Special attack two sequence
        specialAttackTwoSequence.AddChild(specialAttackTwoReady);
        specialAttackTwoSequence.AddChild(startSpecialAttackTwoSequence);

        //Select Special Attacks
        selectSpecialAttack.AddChild(specialAttackTwoSequence);
        selectSpecialAttack.AddChild(specialAttackOneSequence);

        //handling death
        dieSequence.AddChild(dieCondition);
        dieSequence.AddChild(dieStrategy);

        //player death
        doNothingSequence.AddChild(doNothingCondition);
        doNothingSequence.AddChild(doNothingStrategy);

        //Main Selector
        mainSelector.AddChild(doNothingSequence);
        mainSelector.AddChild(dieSequence);
        mainSelector.AddChild(selectSpecialAttack); 
        mainSelector.AddChild(selectNormalAttacks);
        mainSelector.AddChild(chaseSequence);
        mainSelector.AddChild(stayStillStrategy);
       
        //BossX Behaviour Tree
        bossXBehaviourTree.AddChild(mainSelector);

        var noDialogState = new NoDialogueState(this, panel);
        var firstToSayState = new FirstToSayState(this, firstToSay, panel);
        var secondToSayState = new SecondToSayState(this, secondToSay, panel);
        var inAttackToSayState = new InAttackToSay(this, inAttackToSay, panel);

        dialogueStateMachine.currentState = noDialogState;

        At(noDialogState, firstToSayState, new FuncPredicate(() => firstIsReady));
        At(firstToSayState, noDialogState, new FuncPredicate(() => !firstIsReady));
        At(noDialogState, secondToSayState, new FuncPredicate(() => secondIsReady));
        At(secondToSayState, noDialogState, new FuncPredicate(() => !secondIsReady));
        At(noDialogState, inAttackToSayState, new FuncPredicate(() => isInAttackReady));
        At(inAttackToSayState, noDialogState, new FuncPredicate(() => !isInAttackReady));

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x,transform.position.y - distanceToGround));
        Gizmos.DrawLine(upPoint.position, new Vector2(upPoint.position.x + distanceToRight,upPoint.position.y));
        Gizmos.DrawLine(upPoint.position, new Vector2(upPoint.position.x - distanceToLeft, upPoint.position.y));
    }

    private void CollisionDetection()
    {
        isJumped = !Physics2D.Raycast(transform.position, Vector2.down,distanceToGround,ground);
        isUpThere = Physics2D.Raycast(upPoint.position, Vector2.left, distanceToLeft, bossXLayer) ||
            Physics2D.Raycast(upPoint.position, Vector2.right, distanceToRight, bossXLayer);
    }

    private void IgnoreCollision()
    {
        Physics2D.IgnoreLayerCollision(playerController.gameObject.layer, gameObject.layer);
    }

    private void At(IState from, IState to, IPredicate condition)
    {
        dialogueStateMachine.AddTransition(from, to, condition);
    }
    private void Update()
    {

        SetDireaction();
        if(currentHealth <= 0)
        {
            isDead = true;
            
        }

        if (Input.GetKeyDown(KeyCode.L))
            specialTwoCoroutineBlocker = true;

        Debug.Log("special two ready:" + specialTwoCoroutineBlocker);

        if (!bossXAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseRangeAttack")
            && !bossXAnim.GetCurrentAnimatorStateInfo(0).IsName("LongRangeAttack")
            && !bossXAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackAfterDash")
            && !bossXAnim.GetCurrentAnimatorStateInfo(0).IsName("OnLand") && !isDead)
            SetRotation();

        xVelocity = rb.velocity.x;
        dialogueStateMachine.Update();
        
        bossXBehaviourTree.Process();
        CollisionDetection();
        AnimationController();

      
    }

    private void AnimationController()
    {
        bossXAnim.SetFloat("xVelocity", xVelocity);
        bossXAnim.SetFloat("yVelocity", yVelocity);

        bossXAnim.SetBool("isJumped", isJumped);
        bossXAnim.SetBool("isStanceReady", isStanceReady);
        bossXAnim.SetBool("isInLongRangeAttack", isInLongRangeAttack);
        bossXAnim.SetBool("isInCloseRangeAttack", isInCloseRangeAttack);
        bossXAnim.SetBool("onLand", onLand);
        bossXAnim.SetBool("isInSpecialTwo", isSpecialTwoReady);
        bossXAnim.SetBool("isDead", isDead);
    }

    private void SetRotation()
    {
        if (direction.x < 0 && !isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingLeft = true;
        }

        if (direction.x > 0 && isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingLeft = false;
        }
    }

    public void SetDireaction()
    {
        direction = (playerController.transform.position - transform.position).normalized;
        
    }

    public void StartShakingCamera()
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin channel = 
            blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;
        channel.m_AmplitudeGain = 2.5f;
        yield return new WaitForSeconds(0.5f);
        channel.m_AmplitudeGain = 0;
    }

    private IEnumerator GenerateNumberForRandomAttack()
    {
        probOfLongRangeAttack = Random.Range(0, 100);
        isInProgress = true;
        yield return new WaitForSeconds(1f);
        isInProgress = false;
    }

    private IEnumerator GenerateNumberForSpecialAttackOne()
    {
        probOfSpecialOneAttack = Random.Range(0, 100);
        specialOneCoroutineBlocker = true;
        yield return new WaitForSeconds(1f);
        specialOneCoroutineBlocker = false;
    }

    private IEnumerator GenerateNumberForSpecialAttackTwo()
    {
        probOfSpecialTwoAttack = Random.Range(0, 100);
        specialTwoCoroutineBlocker = true;
        yield return new WaitForSeconds(1f);
        specialTwoCoroutineBlocker = false;
    }

    public void EndProgress()
    {
        Debug.Log("Progress Ended");
        isDashAttackInProgress = false;
    }

    private bool IsTooCloseToAWayPoint()
    {
        if(((bossXDistanceToLeftWayPoint < 2f) 
            && (playerController.transform.position.x < (wayPointLeft.transform.position.x + centerPoint))) 
            || ((bossXDistanceToRightWayPoint < 2f) 
            && (playerController.transform.position.x > (wayPointRight.transform.position.x - centerPoint))))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        bossXHealthBar.SetCurrentHealth(currentHealth);

    }

    public float InflictDamage()
    {
        float inflictedDamage = 10f;
        return inflictedDamage;
    }

}
