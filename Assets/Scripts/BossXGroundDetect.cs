using UnityEngine;

public class BossXGroundDetect : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (!GetComponentInParent<BossX>().isDead)
                GetComponentInParent<BossX>().isJumped = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(!GetComponentInParent<BossX>().isDead)
                GetComponentInParent<BossX>().isJumped = true;
        }
    }
}
