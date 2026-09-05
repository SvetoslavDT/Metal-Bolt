using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 1f;
    public float chaseSpeed = 2.5f;
    public int health = 50;
    public float fireRate = 0.5f;

    public float changeDirectionTime = 3f;
    private float patrolTimer;
    private int patrolDirection = 1;

    public float visionDistance = 4f;
    public LayerMask visionMask;

    public GameObject bulletPrefab;

    private bool isAlerted = false;
    private Transform playerTransform;
    private float nextFireTime;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        patrolTimer = changeDirectionTime;
    }

    void Update()
    {
        if (health <= 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            isAlerted = false;
            playerTransform = null;
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isRunning", false);
            }
            return;
        }

        if (!isAlerted)
        {
            CheckForPlayer();
        }

        if (isAlerted && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            spriteRenderer.flipX = (directionToPlayer.x < 0);

            rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool("isMoving", true);
                animator.SetBool("isRunning", true);
            }

            if (Time.time >= nextFireTime)
            {
                Shoot(directionToPlayer);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            patrolTimer -= Time.deltaTime;

            if (patrolTimer <= 0)
            {
                patrolDirection *= -1;
                patrolTimer = changeDirectionTime;
            }

            Vector2 patrolMovement = new Vector2(patrolDirection, 0);
            rb.MovePosition(rb.position + patrolMovement * patrolSpeed * Time.deltaTime);

            spriteRenderer.flipX = (patrolDirection < 0);

            if (animator != null)
            {
                animator.SetBool("isMoving", true);
                animator.SetBool("isRunning", false);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health > 0) AlertEnemy();
        else Die();
    }

    public void TakeMeleeHit()
    {
        if (!isAlerted)
        {
            Debug.Log("Stealth Kill!");
            health = 0;
            Die();
        }
        else
        {
            TakeDamage(30);
        }
    }

    public void AlertEnemy()
    {
        if (!isAlerted)
        {
            isAlerted = true;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayAlert();
                AudioManager.instance.SwitchToAlertMusic();
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

            RoomSpawner spawner = Object.FindAnyObjectByType<RoomSpawner>();
            if (spawner != null)
            {
                spawner.isRoomAlerted = true;
            }

            EnemyAI[] allEnemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (EnemyAI enemy in allEnemies)
            {
                if (enemy != null && !enemy.isAlerted)
                {
                    enemy.isAlerted = true;
                    if (player != null) enemy.playerTransform = player.transform;
                }
            }
        }
    }


    void Die()
    {
        if (animator != null) animator.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null) boxCollider.enabled = false;

        GameHUD.AddPoints(10);

        RoomSpawner spawner = Object.FindAnyObjectByType<RoomSpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyKilled(isAlerted);
        }

        Destroy(gameObject, 2f);
    }

    void Shoot(Vector2 direction)
    {
        if (bulletPrefab != null)
        {
            if (animator != null) animator.SetTrigger("shoot");

            GameObject bulletInstance = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Setup(direction, true);
            }
        }
    }

    void CheckForPlayer()
    {
        Vector2 lookDirection = new Vector2(patrolDirection, 0f);

        Vector2 rayOrigin =
            (Vector2)transform.position + lookDirection * 0.3f;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            lookDirection,
            visionDistance,
            visionMask
        );

        Debug.DrawRay(
            rayOrigin,
            lookDirection * visionDistance,
            Color.cyan
        );

        if (hit.collider == null)
            return;

        Debug.Log("Raycast hit: " +
                  hit.collider.name +
                  " | Tag: " +
                  hit.collider.tag);

        if (hit.collider.CompareTag("Player"))
        {
            Debug.Log("PLAYER DETECTED!");
            AlertEnemy();
        }
        else if (hit.collider.CompareTag("Wall"))
        {
            Debug.Log("Wall is blocking vision.");
        }
    }

}
