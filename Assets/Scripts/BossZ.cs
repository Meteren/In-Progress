
using UnityEngine;

public class BossZ : Boss
{
    private BehaviourTree bossZTree;
    public Transform centerPoint;

    [Header("Conditions")]
    public bool canMeleeAttack;
    public bool firstReadyToSay = false;

    void Start()
    {
        GameManager.instance.blackBoard.SetValue("BossZ", this);
        InitBehaviourTree();
        if(playerController is null)
        {
            Debug.Log("Controller null");
        }
        IgnoreCollision();

        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");

        Leaf stayStillStrategy = new Leaf("StayStillStrategy", new StayStillStrategy(this),40);

        SequenceNode meleeAttackSequence = new SequenceNode("MeleeAttackSequence",30);
        Leaf meleeAttackCondition = new Leaf("MeleeCondition",new Condition(() => firstReadyToSay));

        meleeAttackSequence.AddChild(meleeAttackCondition);
        
        SequenceNode processAttackSequence = new SequenceNode("ProcessAttackSequence");
        meleeAttackSequence.AddChild(processAttackSequence);

        Leaf moveToPlayerStrategy = new Leaf("MoveToPlayerStrategy",
            new HandleMovementStrategy(HandleMovementStrategy.State.moveToPlayerOffset, Vector2.zero, 25f));
        Leaf attackStrategy = new Leaf("AttackStrategy", new MeleeAttackStrategy());
        Leaf getBackStrategy =
            new Leaf("GetBackStrategy",
            new HandleMovementStrategy(HandleMovementStrategy.State.moveToChoosedPos, centerPoint.transform.position, 25f));

        processAttackSequence.AddChild(moveToPlayerStrategy);
        processAttackSequence.AddChild(attackStrategy);
        processAttackSequence.AddChild(getBackStrategy);

        mainSelector.AddChild(stayStillStrategy);
        mainSelector.AddChild(meleeAttackSequence);

        bossZTree.AddChild(mainSelector);


    }

    private void FixedUpdate()
    {
        bossZTree.Process();
    }
    void Update()
    {
        SetDireaction();

        if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("melee_attack"))
            SetBossZRotation();

        AnimationController();

        if (Input.GetKeyDown(KeyCode.O))
        {
            firstReadyToSay = !firstReadyToSay;
        }

    }

    private void SetBossZRotation()
    {
        if (direction.x > 0 && !isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingLeft = true;
        }

        if (direction.x < 0 && isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingLeft = false;
        }
    }
    public override void AnimationController()
    {
        bossAnim.SetBool("meleeAttack", canMeleeAttack);
    }

    public override float InflictDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void InitBehaviourTree()
    {
        bossZTree = new BehaviourTree("BossZ");
    }

    public override void OnDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
 
}
