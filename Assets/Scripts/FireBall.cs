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
            PlayerController controller = GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
            if (controller != null)
            {
                Debug.Log("Not Null");
            }
            if (collision.gameObject.GetComponent<BossX>() != null && !collision.gameObject.GetComponent<BossX>().isDead && !controller.isDead)
            {
                BossX bossX = collision.gameObject.GetComponent<BossX>();
                bossX.OnDamage(InflictDamage());
                SpriteRenderer bossXRenderer = bossX.bossRenderer;
                Vector2 contactPoint = collision.ClosestPoint(transform.position);
                Vector2 oppositeDirection = new Vector2(-1 * Direction.x, Direction.y);
                HitParticle clonedHitParticle = ObjectPooling.DequeuePool<HitParticle>("HitParticle");
                clonedHitParticle.Init(oppositeDirection, contactPoint, bossXRenderer);
                ObjectPooling.EnqueuePool("FireBall", this);
            }
            
        }

    }

    private float InflictDamage()
    {
        float inflictedDamage = 1f;
        return inflictedDamage;
    }
}