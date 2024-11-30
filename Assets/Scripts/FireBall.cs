using UnityEngine;

public class FireBall : MonoBehaviour
{

    [SerializeField] private float speed = 10f;
    [SerializeField] private Rigidbody2D rb;
    float angle;
    public Vector2 Direction { get; private set; }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initiliaze(Vector2 position, Vector2 direction, float angle)
    {
        this.Direction = direction;
        this.angle = angle;
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        rb.velocity = Vector2.zero;
        gameObject.SetActive(true);
    }

    void Update()
    {
        rb.velocity = Direction.normalized * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            PlayerController _controller = GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController __controller) ? __controller : null;
           
            if (collision.gameObject.TryGetComponent<Boss>(out Boss boss) && !boss.isDead && !_controller.isDead)
            {
                boss.OnDamage(InflictDamageToBoss());
                SpriteRenderer bossRenderer = boss.bossRenderer;
                Vector2 oppositeDirection = new Vector2(-1 * Direction.x, Direction.y);
                InitFireBallImpact(collision,oppositeDirection,bossRenderer);
                
            }
           
            
        }

        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller) && !controller.isDead)
        {
            if (!controller.isInDash)
            {
                Gun gun = GameObject.Find("Gun").GetComponent<Gun>();
                if (!gun.isBelongToPlayer)
                {
                    if (controller.ledgeDetected)
                    {

                        controller.transform.position = controller.tracker.transform.position;

                    }
                    controller.isDamaged = true;
                    controller.OnDamage(InflictDamageToPlayer());
                    Vector2 direction = new Vector2(-1 * Direction.x, Direction.y);
                    if (controller.isJumped)
                    {
                        controller.damageDirection = direction;
                    }
                    else
                    {
                        controller.damageDirection = Vector2.zero;
                    }

                    InitFireBallImpact(collision, direction);

                }
            }
            
        }

        if (collision.gameObject.TryGetComponent<SummonedSpirit>(out SummonedSpirit spirit))
        {
            Vector2 oppositeDirection = new Vector2(-1 * Direction.x, Direction.y);
            InitFireBallImpact(collision, oppositeDirection);
            if (!spirit.isAttached)
            {
                Destroy(spirit.gameObject);
            }

        }

    }

    private void InitFireBallImpact(Collider2D collision,Vector2 direction, SpriteRenderer bossRenderer = null)
    {
        Vector2 contactPoint = collision.ClosestPoint(transform.position);
        HitParticle clonedHitParticle = ObjectPooling.DequeuePool<HitParticle>("HitParticle");
        ObjectPooling.EnqueuePool<FireBall>("FireBall", this);
        clonedHitParticle.Init(direction, contactPoint, bossRenderer);
    }

    private float InflictDamageToBoss()
    {
        float inflictedDamage = 1f;
        return inflictedDamage;
    }

    private float InflictDamageToPlayer()
    {
        return 5f;
    }
}