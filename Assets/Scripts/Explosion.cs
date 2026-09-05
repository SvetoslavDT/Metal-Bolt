using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 75;

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            EnemyAI enemy = hitInfo.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Debug.Log("Enemy was hit!");
        }
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}