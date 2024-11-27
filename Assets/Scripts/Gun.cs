using Cinemachine;
using UnityEngine;

public class Gun : MonoBehaviour
{
    Vector3 mousePosition = Vector2.zero;
    float angle;
    Vector2 direction;
    [SerializeField] private float offset;
    private PlayerController controller => 
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null; 
    [SerializeField] private ParticleSystem gunParticle;
    float timer = 0.4f;
    bool isTimerInProgress = false;
    public bool isBelongToPlayer = true;
    bool isPositionSetted = true;
    [SerializeField] private ParticleSystem gunExplosionEffect;
    private CinemachineBasicMultiChannelPerlin channel =>
        GameManager.instance.blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;

    float rePositioningSpeed = 7f; 
    void Update()
    {
        if (!controller.isDead)
        {
            if (isBelongToPlayer)
            {
                if (!isPositionSetted)
                {
                    transform.position = 
                        Vector2.MoveTowards(transform.position, controller.transform.position, Time.deltaTime * rePositioningSpeed);
                    if(Vector2.Distance(controller.transform.position,transform.position) <= offset)
                    {
                        isPositionSetted = true;
                    }

                }
                else
                {

                    HandleGunRotation();
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Shoot();
                    }
                }


            }
           
            
        }
        else
        {  
            gunExplosionEffect.transform.position = transform.position;
            Destroy(gameObject);
            gunExplosionEffect.Play();
    
        }
        
       
        if(isTimerInProgress)
            timer -= Time.deltaTime;
        if (timer <= 0)
        {
            channel.m_AmplitudeGain = 0f;
            timer = 0.4f;
            isTimerInProgress = false;
        }
    }

    private void HandleGunRotation()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (mousePosition - controller.transform.position);
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = controller.transform.position + Quaternion.Euler(0, 0, angle + 90) * new Vector3(offset, 0, 0);
    }

    public void Shoot()
    {
       
        isTimerInProgress = true;
        channel.m_AmplitudeGain = 0;
        timer = 0.4f;
        channel.m_AmplitudeGain = 2.5f;
        gunParticle.Play();
        FireBall fireBall = ObjectPooling.DequeuePool<FireBall>("FireBall");
        fireBall.Initiliaze(transform.position, direction, angle);
        
    }

   
}
