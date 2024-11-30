using UnityEngine;

public class HitParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;
    float duration;
    float hitRenderTime;
    SpriteRenderer bossRenderer;
    bool isTakeDamageInProgress = false;
    float startingTime;
    float finishingTime;
    void Start()
    {
        duration = hitParticle.main.duration;
        hitRenderTime = duration - 0.1f;
    }

    public void Init(Vector2 direction,Vector2 position,SpriteRenderer bossRenderer  = null)
    {
        startingTime = Time.time;
        finishingTime = startingTime + hitRenderTime;
        
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle);
        transform.position = position;
        gameObject.SetActive(true);
        hitParticle.Play();
        if (bossRenderer != null)
        {
            this.bossRenderer = bossRenderer;
            bossRenderer.color = Color.red;
        }
        
       
        
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if(bossRenderer != null)
        {
            if (duration < hitRenderTime && !isTakeDamageInProgress)
            {
                bossRenderer.color = Color.white;
                isTakeDamageInProgress = true;
            }
        }

        if(duration <= 0)
        {
            ObjectPooling.EnqueuePool("HitParticle", this);
            duration = hitParticle.main.duration;
            isTakeDamageInProgress = false;
        }
    }
}
