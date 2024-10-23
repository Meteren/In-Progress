using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Gun : MonoBehaviour
{
    Vector3 mousePosition = Vector2.zero;
    float angle;
    Vector2 direction;
    [SerializeField] private float offset;
    [SerializeField] private PlayerController controller;
    [SerializeField] private ParticleSystem gunParticle;
    float timer = 0.4f;
    bool isTimerInProgress = false;
    [SerializeField] private ParticleSystem gunExplositonEffect;
    private CinemachineBasicMultiChannelPerlin channel =>
        GameManager.instance.blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;

    void Update()
    {
        if (!controller.isDead)
        {
            HandleGunRotation();
            Shoot();
        }
        else
        {  
            gunExplositonEffect.transform.position = transform.position;
            Destroy(gameObject);
            gunExplositonEffect.Play();
    
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

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
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

   
}
