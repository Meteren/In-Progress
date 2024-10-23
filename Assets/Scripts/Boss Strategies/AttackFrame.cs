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
                
                if(collision.gameObject.GetComponent<PlayerController>().stateMachine.currentState is not DamageState)
                {
                    Debug.Log("Hit");
                    BossX bossX = GetComponentInParent<BossX>() ?? GameObject.Find("BossX").GetComponent<BossX>();
                    collision.gameObject.GetComponent<PlayerController>().isDamaged = true;
                    collision.gameObject.GetComponent<PlayerController>().OnDamage(bossX.InflictDamage());
                }
                
            }
            
            
        }
    }
}
