using AdvancedStateHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    protected PlayerController playerController
        => GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    [SerializeField] protected LayerMask ground;
    [SerializeField] protected LayerMask bossLayer;
    public SpriteRenderer bossRenderer;

    public Animator bossAnim;

    public Vector2 direction;

    public float xVelocity;
    public float yVelocity;

    protected BehaviourTree bossBehaviourTree;
    protected AdvancedStateMachine dialogueStateMachine = new AdvancedStateMachine();
    protected BlackBoard blackBoard => GameManager.instance.blackBoard;

    public float distanceToPlayer => Vector2.Distance(transform.position, playerController.transform.position);

    [Header("Health")]
    public HealthBar bossHealthBar;
    public float currentHealth;
    public float maxHealth = 100;

    [Header("Rb")]
    public Rigidbody2D rb;

    [Header("Conditions")]
    public bool isDead = false;
    public bool isFacingLeft = true;
    public bool isInCloseRangeAttack = false;
    public bool isInLongRangeAttack = false;

    public abstract void OnDamage(float damageAmount);

    public abstract float InflictDamage();

    public abstract void InitBehaviourTree();

    public abstract void AnimationController();

    protected void IgnoreCollision()
    {
        Physics2D.IgnoreLayerCollision(playerController.gameObject.layer, gameObject.layer);
    }

    protected void SetRotation()
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

}
