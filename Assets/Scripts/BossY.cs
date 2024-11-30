using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BossY : Boss
{
    public bool firstToSay = false;
    public bool activateSpecialOne = false;
    public bool activateSkill;
    public bool summonAttack;
    public bool canSummon;
    public bool blockSpecialOneCoroutine = false;
    public bool specialTwoReady = false;
    public bool isSpecialTwoFinished = false;

    new public float distanceToPlayer =>
        Vector2.Distance(new Vector2(playerController.transform.position.x, 0), new Vector2(transform.position.x, 0)); 
    
    public Vector3 previousLocation;
     
    public Collider2D generationFrame;

    float probabilityOfSpecialOne;

    public Gun gun;

    public Queue<SummonedSpirit> offenseSpirits = new Queue<SummonedSpirit>();
    public List<SummonedSpirit> defenseSpirits = new List<SummonedSpirit>();

    [Header("Points")]
    [SerializeField] private Transform specialPoint;
    [SerializeField] private Transform patrolLineOne;
    [SerializeField] private Transform patrolLineTwo;
    public Transform centerPoint;
    public Transform focusPoint;
    public Transform upperPoint;
    public List<Transform> gunPoints;

    Transform closestPatrolPointToPlayer =>
        Vector2.Distance(playerController.transform.position, patrolLineOne.transform.position) >
        Vector2.Distance(playerController.transform.position, patrolLineTwo.transform.position) ? patrolLineTwo : patrolLineOne;

    Transform farestPatrolPointToPlayer => 
        Vector2.Distance(playerController.transform.position, patrolLineOne.transform.position) >
        Vector2.Distance(playerController.transform.position, patrolLineTwo.transform.position) ? patrolLineOne : patrolLineTwo;

    public CinemachineBasicMultiChannelPerlin wideChannel =>
        GameManager.instance.blackBoard.GetValue("WideChannel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;

    public CinemachineVirtualCamera wideCam;

    [Header("Prefab")]
    public SummonedSpirit referenceSpirit;

    [Header("Level Controller")]
    public BossYLevelController levelController;
    void Start()
    {
        InitBehaviourTree();
        IgnoreCollision();

        blackBoard.SetValue("BossY", this);

        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");

        //Stay Still
        Leaf stayStillStrategy = new Leaf("StayStillStrategy",new StayStillStrategy(this),40);

        //Chase Sequence
        SequenceNode chaseSequence = new SequenceNode("ChaseSequence",30);
        Leaf chaseCondition = new Leaf("ChaseCondition", new Condition(() => firstToSay));
        Leaf chasePlayer = new Leaf("ChasePlayer", new ChasePlayerStrategy(this,7), 0);
        chaseSequence.AddChild(chaseCondition);
        chaseSequence.AddChild(chasePlayer);

        //long range attack
        SequenceNode longRangeAttackSequence = new SequenceNode("LongRangeAttackSequence",10);
        Leaf longRangeAttackCondition = new Leaf("LongRangeAttackCondition", new Condition(
            () => distanceToPlayer <= 2.8f && distanceToPlayer > 2f ));
        Leaf longRangeAttackStrategy = new Leaf("LongRangeAttackStrategy", new LongRangeAttackStrategyForBossY());
        longRangeAttackSequence.AddChild(longRangeAttackCondition);
        longRangeAttackSequence.AddChild(longRangeAttackStrategy);
        //close range attack
        SequenceNode closeRangeAttackSequence = new SequenceNode("CloseRangeAttackSequence",20);
        Leaf closeRangeAttackCondition = new Leaf("closeRangeAttackCondition", new Condition(
           () => distanceToPlayer <= 2f));
        Leaf closeRangeAttackStrategy = new Leaf("LongRangeAttackStrategy", new CloseRangeAttackStrategyForBossY());
        closeRangeAttackSequence.AddChild(closeRangeAttackCondition);
        closeRangeAttackSequence.AddChild(closeRangeAttackStrategy);

        //special long range
        SequenceNode specialLRASequence = new SequenceNode("SpecialLongRangeAttackSequence",5);
        Leaf specialLRACondition = new Leaf("SpecialLongRangeCondition", new Condition(()
            => distanceToPlayer < 5f && distanceToPlayer > 2.8f));
        Leaf getCloseStrategy = new Leaf("GetCloseStrategy", new GetCloseStrategy());
        Leaf getAwayStrategy = new Leaf("GetAwayStrategy", new GetAwayStrategy());

        //special one
        SortedSelectorNode specialAttackSelector = new SortedSelectorNode("SpecialAttackSelector",10);
        SequenceNode specialAttackOneSequence = new SequenceNode("SpecialAttackOneSequence",20);

        Leaf specialOneCondition = new Leaf("SpecialOneCondition",new Condition(() =>
        {
            if(!blockSpecialOneCoroutine)
                StartCoroutine(SpecialOneProbability());

            return probabilityOfSpecialOne >= 85;
        }));

        SequenceNode processSpecialAttackOneSequence = new SequenceNode("ProcessSpecialAttackOneSequence");

        Leaf moveToSpecialPointStrategy = new Leaf("MoveToSpecialPointStrategy", new MoveToPointStrategy(specialPoint,13f,false));
        Leaf setAttachedSpiritsAroundStrategy = new Leaf("SetAttachedSpiritsAroundStrategy", new SetAttachedSpiritsAroundStrategy());
        Leaf shootSpiritStrategy = new Leaf("ShootSpiritStrategy", new ShootSpiritsStrategy());

        processSpecialAttackOneSequence.AddChild(moveToSpecialPointStrategy);
        processSpecialAttackOneSequence.AddChild(setAttachedSpiritsAroundStrategy);
        processSpecialAttackOneSequence.AddChild(shootSpiritStrategy);

        SortedSelectorNode selectEndingSituation = new SortedSelectorNode("SelectEndingSituation");

        specialAttackOneSequence.AddChild(specialOneCondition);
        specialAttackOneSequence.AddChild(processSpecialAttackOneSequence);
        specialAttackOneSequence.AddChild(selectEndingSituation);

        SequenceNode endingOneSequence = new SequenceNode("EndingOneSequence", 10);
        Leaf moveToClosestPatrolPointToPlayerStrategy = new Leaf("MoveToClosestPatrolPointToPlayerStrategy",
           new MoveToPointStrategy(closestPatrolPointToPlayer, 13f,false), 20);


        selectEndingSituation.AddChild(endingOneSequence);
        selectEndingSituation.AddChild(moveToClosestPatrolPointToPlayerStrategy);


        Leaf endingOneCondition = new Leaf("EndingOneCondition", new Condition(() =>
        {   
            int randomNumber = Random.Range(0, 100);
            return randomNumber >= 40;
        }));

        SequenceNode processEndingOneSequence = new SequenceNode("ProcessEndingOneSequence");

        endingOneSequence.AddChild(endingOneCondition);
        endingOneSequence.AddChild(processEndingOneSequence);

        Leaf moveToPlayerStrategy = new Leaf("MoveToPlayerStrategy", new MoveToPointStrategy(focusPoint, 24f,false));
        Leaf needleAttackStrategy = new Leaf("NeedleAttackStrategy", new NeedleAttackStrategy());
        Leaf moveToFarestPatrolPoint = new Leaf("MoveToFarestPatrolPoint",
            new MoveToPointStrategy(farestPatrolPointToPlayer,13f,false));

        processEndingOneSequence.AddChild(moveToPlayerStrategy);
        processEndingOneSequence.AddChild(needleAttackStrategy);
        processEndingOneSequence.AddChild(moveToFarestPatrolPoint);

        //special two
        SequenceNode specialAttackTwoSequence = new SequenceNode("SpecialAttackTwoSequence",10);

        Leaf specialAttackTwoCondition = new Leaf("SpecialAttackTwoCondition",new Condition(() => specialTwoReady));

        specialAttackTwoSequence.AddChild(specialAttackTwoCondition);

        SequenceNode processSpecialTwoSequence = new SequenceNode("ProcessSpecialTwoSequence");

        specialAttackTwoSequence.AddChild(processSpecialTwoSequence);

        Leaf getGunAndMoveUpStrategy = new Leaf("GetGunAndMoveUpStrategy", new GetGunAndMoveUpStrategy());
        Leaf sendGunToPointAndRotateStrategy = new Leaf("SendGunToPointAndRotateStrategy",new SendGunToPointAndRotateStrategy());
        Leaf changeGunPositionAndShootStrategy = new Leaf("ChangeGunPositionAndShootStrategy", new ChangeGunPositionAndShootStrategy());
        Leaf moveToClosestWayPointFasterStrategy =
            new Leaf("MoveToClosestWayPointFasterStrategy", new MoveToPointStrategy(closestPatrolPointToPlayer, 20f,false));

        processSpecialTwoSequence.AddChild(getGunAndMoveUpStrategy);
        processSpecialTwoSequence.AddChild(sendGunToPointAndRotateStrategy);
        processSpecialTwoSequence.AddChild(changeGunPositionAndShootStrategy);
        processSpecialTwoSequence.AddChild(moveToClosestWayPointFasterStrategy);

        //Addings
        specialAttackSelector.AddChild(specialAttackTwoSequence);
        specialAttackSelector.AddChild(specialAttackOneSequence);

        specialLRASequence.AddChild(specialLRACondition);
        specialLRASequence.AddChild(getCloseStrategy);
        specialLRASequence.AddChild(longRangeAttackStrategy);
        specialLRASequence.AddChild(getAwayStrategy);

        //attack selector
        SortedSelectorNode attackSelector = new SortedSelectorNode("AttackSelector", 20);
        attackSelector.AddChild(specialLRASequence);
        attackSelector.AddChild(longRangeAttackSequence);
        attackSelector.AddChild(closeRangeAttackSequence);

        //main selector
        mainSelector.AddChild(specialAttackSelector);
        mainSelector.AddChild(attackSelector);
        mainSelector.AddChild(chaseSequence);
        mainSelector.AddChild(stayStillStrategy);
        
        bossBehaviourTree.AddChild(mainSelector);
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        bossBehaviourTree.Process();
    }
    void Update()
    {
        SetDireaction();
        ListenSpirits();
        if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseRangeAttack")
            && !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("LongRangeAttack"))
            SetRotation();

        if (Input.GetKeyDown(KeyCode.B))
            firstToSay = true;
       
        AnimationController();

        if (Input.GetKeyDown(KeyCode.O))
        {
            specialTwoReady = !specialTwoReady;
        }
  
    }

    public override void AnimationController()
    {
        bossAnim.SetBool("isInLongRangeAttack", isInLongRangeAttack);
        bossAnim.SetBool("isInCloseRangeAttack", isInCloseRangeAttack);
        bossAnim.SetBool("activateSkill", activateSkill);
        bossAnim.SetBool("summonAttack", summonAttack);
        bossAnim.SetBool("canSummon", canSummon);
    }

    public override float InflictDamage()
    {
        return 10f;
    }

    public override void InitBehaviourTree()
    {
        bossBehaviourTree = new BehaviourTree("BossY");
    }

    public override void OnDamage(float damageAmount)
    {
        Debug.Log("Damage");
    }

    public IEnumerator SpecialOneProbability()
    {
        probabilityOfSpecialOne = Random.Range(0, 100);
        blockSpecialOneCoroutine = true;
        yield return new WaitForSeconds(1f);
        blockSpecialOneCoroutine = false;
        

    }

    private void ListenSpirits()
    {
        Queue<SummonedSpirit> tempOffenseQueue = 
            new Queue<SummonedSpirit>(offenseSpirits.Where(spirit => !spirit.IsDestroyed()));

        List<SummonedSpirit> tempDefenseQueue = new List<SummonedSpirit>(defenseSpirits.Where(spirit => !spirit.IsDestroyed()));

        offenseSpirits = tempOffenseQueue;
        defenseSpirits = tempDefenseQueue;
       
    }

    public IEnumerator ShakeWideCamera()
    {    
        wideChannel.m_AmplitudeGain = 2.5f;
        yield return new WaitForSeconds(0.5f);
        wideChannel.m_AmplitudeGain = 0;
    }


}
