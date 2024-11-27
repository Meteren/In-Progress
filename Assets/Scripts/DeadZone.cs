using UnityEngine;

public class DeadZone : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<FireBall>() != null)
        {
            ObjectPooling.EnqueuePool("FireBall", collision.GetComponent<FireBall>());
        }
    }
  

}
