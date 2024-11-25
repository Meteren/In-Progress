using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossY : Boss
{
    
    public bool firstToSay = false;
    public Vector3 previousLocation;
    [Header("Prefab")]
    [SerializeField] private SummonedSpirit referenceSpirit;
    public Transform centerPoint;
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
            () => distanceToPlayer < 2.8f && distanceToPlayer > 2f ));
        Leaf longRangeAttackStrategy = new Leaf("LongRangeAttackStrategy", new LongRangeAttackStrategyForBossY());
        longRangeAttackSequence.AddChild(longRangeAttackCondition);
        longRangeAttackSequence.AddChild(longRangeAttackStrategy);
        //close range attack
        SequenceNode closeRangeAttackSequence = new SequenceNode("CloseRangeAttackSequence",20);
        Leaf closeRangeAttackCondition = new Leaf("closeRangeAttackCondition", new Condition(
           () => distanceToPlayer < 2f));
        Leaf closeRangeAttackStrategy = new Leaf("LongRangeAttackStrategy", new CloseRangeAttackStrategyForBossY());
        closeRangeAttackSequence.AddChild(closeRangeAttackCondition);
        closeRangeAttackSequence.AddChild(closeRangeAttackStrategy);

        //special long range
        SequenceNode specialLRASequence = new SequenceNode("SpecialLongRangeAttackSequence",5);
        Leaf specialLRACondition = new Leaf("SpecialLongRangeCondition", new Condition(()
            => distanceToPlayer < 5f && distanceToPlayer > 2.8f));
        Leaf getCloseStrategy = new Leaf("GetCloseStrategy", new GetCloseStrategy());
        Leaf getAwayStrategy = new Leaf("GetAwayStrategy", new GetAwayStrategy());

        specialLRASequence.AddChild(specialLRACondition);
        specialLRASequence.AddChild(getCloseStrategy);
        specialLRASequence.AddChild(longRangeAttackStrategy);
        specialLRASequence.AddChild(getAwayStrategy);

        //attack selector
        SortedSelectorNode attackSelector = new SortedSelectorNode("AttackSelector", 20);
        attackSelector.AddChild(specialLRASequence);
        attackSelector.AddChild(longRangeAttackSequence);
        attackSelector.AddChild(closeRangeAttackSequence);


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
        if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseRangeAttack")
            && !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("LongRangeAttack"))
            SetRotation();

        if (Input.GetKeyDown(KeyCode.B))
            firstToSay = true;
       
        AnimationController();

        if (Input.GetKeyDown(KeyCode.O))
        {
            float angle = 0;
            float incrementAmount = 45;
            for(int i = 0; i < 8; i++)
            {
                SummonedSpirit defenseSpirit = Instantiate(referenceSpirit);
                SummonedSpirit offenseSpirit = Instantiate(referenceSpirit);
                defenseSpirit.Init(centerPoint, angle * Mathf.Deg2Rad,centerPoint.GetComponent<WayPoint>().radius,false);
                offenseSpirit.Init(centerPoint, angle * Mathf.Deg2Rad, 0, true);
                angle += incrementAmount;
            }
        }
    }

    public override void AnimationController()
    {
        bossAnim.SetBool("isInLongRangeAttack", isInLongRangeAttack);
        bossAnim.SetBool("isInCloseRangeAttack", isInCloseRangeAttack);
    }

    public override float InflictDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void InitBehaviourTree()
    {
        bossBehaviourTree = new BehaviourTree("BossY");
    }

    public override void OnDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }

    
}
