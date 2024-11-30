using UnityEngine;

public class AttackFrame : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if (collision.gameObject.GetComponent<PlayerController>().isInDash)
            {
                Debug.Log("Avoided!!!");
            }
            else
            {
                Debug.Log("Coll");
                if(collision.gameObject.GetComponent<PlayerController>().stateMachine.currentState is not DamageState)
                {
                    PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
                    Debug.Log("Hit");
                    Boss boss = GetComponentInParent<Boss>() ?? GameObject.Find("BossX").GetComponent<Boss>();
                    controller.isDamaged = true;
                    controller.OnDamage(boss.InflictDamage());
                    controller.damageDirection = GetComponentInParent<Boss>().direction;
                }
                
            }


        }
        else
        {
            Debug.Log("Player Null");
        }
    }
}
