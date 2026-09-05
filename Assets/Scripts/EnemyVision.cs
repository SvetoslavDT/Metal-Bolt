using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private EnemyAI parentAI;
    public LayerMask obstacleMask;

    void Start()
    {
        parentAI = GetComponentInParent<EnemyAI>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 directionToPlayer = (other.transform.position - transform.parent.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.parent.position, other.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.parent.position, directionToPlayer, distanceToPlayer, obstacleMask);

            if (hit.collider == null)
            {
                Debug.Log("Player detected!");
                if (parentAI != null)
                {
                    parentAI.AlertEnemy();
                }
            }
            else
            {
                Debug.Log("Snake is behind a wall.");
            }
        }
    }
}