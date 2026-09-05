using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int baseDamage = 20;
    private Vector2 moveDirection;
    private bool isEnemyBullet;

    public void Setup(Vector2 direction, bool shotByEnemy)
    {
        moveDirection = direction;
        isEnemyBullet = shotByEnemy;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(gameObject, 2f);
    }

void FixedUpdate()
{
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.isTrigger)
        {
            return;
        }

        if (isEnemyBullet && hitInfo.CompareTag("Player"))
        {
            PlayerController player = hitInfo.GetComponent<PlayerController>();
            if (player != null)
            {
                int finalDamage = CalculateDifficultyDamage(baseDamage);
                player.TakeDamage(finalDamage);
            }
            Destroy(gameObject);
        }
        else if (!isEnemyBullet && hitInfo.CompareTag("Enemy"))
        {
            EnemyAI enemy = hitInfo.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
            }
            Destroy(gameObject);
        }
        else if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    int CalculateDifficultyDamage(int damage)
    {
        if (DifficultyManager.instance != null)
        {
            string currentDiff = DifficultyManager.instance.currentDifficulty;
            if (currentDiff == "Easy") return Mathf.RoundToInt(damage * 0.5f);
            if (currentDiff == "Normal") return Mathf.RoundToInt(damage * 1f);
            if (currentDiff == "Hard") return Mathf.RoundToInt(damage * 2f);
        }
        return damage;
    }
}
