using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastFacingDirection = Vector2.right;

    public int maxHealth = 100;
    public int currentHealth;
    public int grenadeCount = 3;

    public int ammoCount = 30;
    public int reserveAmmo = 30;
    private int maxMagazineSize = 30;

    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public float fireRate = 0.2f;
    public float grenadeDistance = 2f;
    private float nextFireTime;

    public float meleeRange = 1.2f;
    public float meleeRadius = 0.5f;

    private bool isActing = false;

    private static PlayerController instance;

    void Start()
    {
        currentHealth = maxHealth;
        ammoCount = 30;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetInteger("health", currentHealth);
            anim.SetBool("isMoving", false);
            anim.SetBool("isRunning", false);
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        bool isHoldingSpace = Keyboard.current != null && Keyboard.current.spaceKey.isPressed && ammoCount > 0 && !isActing;

        float x = 0; float y = 0;

        if (!isHoldingSpace && !isActing && Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1;

            if (Keyboard.current.kKey.wasPressedThisFrame) ThrowGrenade();
            if (Keyboard.current.lKey.wasPressedThisFrame) MeleeAttack();
            if (Keyboard.current.rKey.wasPressedThisFrame) Reload();
        }

        movement = new Vector2(x, y);

        if (movement.x < 0) { spriteRenderer.flipX = true; lastFacingDirection = Vector2.left; }
        else if (movement.x > 0) { spriteRenderer.flipX = false; lastFacingDirection = Vector2.right; }

        bool isMoving = (movement.x != 0 || movement.y != 0);
        bool isShiftPressed = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool isRunning = isMoving && isShiftPressed;

        if (isRunning) moveSpeed = 4f;
        else moveSpeed = 2f;

        if (isHoldingSpace && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isShooting", isHoldingSpace);
            animator.SetInteger("health", currentHealth);
        }
    }
    void FixedUpdate()
    {
        bool isHoldingSpace = Keyboard.current != null && Keyboard.current.spaceKey.isPressed && ammoCount > 0;

        if (isHoldingSpace || isActing || currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (movement.magnitude > 0)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Shoot()
    {
        if (AudioManager.instance != null) 
        { 
            AudioManager.instance.PlayPlayerShoot();
        }


        ammoCount--;
        Debug.Log("Shooting! Ammo: " + ammoCount);

        if (bulletPrefab != null)
        {
            Vector3 bulletSpawnPosition = transform.position + new Vector3(0f, 0.2f, 0f);

            GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);

            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Setup(lastFacingDirection, false);
            }
        }
    }

    void ThrowGrenade()
{
    if (grenadeCount > 0)
    {
        grenadeCount--;
        isActing = true;
        rb.linearVelocity = Vector2.zero;
        
        if (animator != null) animator.SetTrigger("throwGrenade");
        Debug.Log("Grenade thown! Left : " + grenadeCount);
    }
}

    public void SpawnExplosionEvent()
    {
        Vector3 explosionPosition = transform.position + (Vector3)(lastFacingDirection * grenadeDistance);

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayGrenade();
        }
    }


    void MeleeAttack()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMeleeAttack();
        }

        isActing = true;
        rb.linearVelocity = Vector2.zero;
        if (animator != null) 
        {
            animator.SetTrigger("melee");
        }
        Invoke("ResetAction", 0.3f);

        Vector2 attackPoint = (Vector2)transform.position + (lastFacingDirection * meleeRange);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, meleeRadius);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyAI enemy = enemyCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeMeleeHit();
                }
            }
        }
    }

    void Reload()
    {
        if (ammoCount >= maxMagazineSize || reserveAmmo <= 0)
        {
            isActing = false;
            return;
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayReload();
        }

        isActing = true;
        rb.linearVelocity = Vector2.zero;
        if (animator != null)
        {
            animator.SetTrigger("reload");
        }

        int ammoNeeded = maxMagazineSize - ammoCount;

        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        ammoCount += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        Debug.Log("Reloading! In mag: " + ammoCount + " | In reserve: " + reserveAmmo);

        Invoke("ResetAction", 1.0f);
    }


    public void ResetAction()
    {
        isActing = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetInteger("health", currentHealth);

        if (currentHealth <= 0)
        {
            if (AudioManager.instance != null)
            {
                if (AudioManager.instance.normalGameMusic != null) AudioManager.instance.normalGameMusic.Stop();
                if (AudioManager.instance.alertGameMusic != null) AudioManager.instance.alertGameMusic.Stop();
                AudioManager.instance.PlayDie();
            }

            Die();

            GameOverMenu gameOverMenu = Object.FindAnyObjectByType<GameOverMenu>();
            if (gameOverMenu != null)
            {
                gameOverMenu.TriggerGameOver();
            }
        }
    }
    public static void ResetPlayerInstance()
    {
        instance = null;
    }

    void Die()
    {
        if (animator != null) animator.SetTrigger("die");

        gameObject.tag = "Untagged";

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null) boxCollider.enabled = false;

        this.enabled = false;
        rb.linearVelocity = Vector2.zero;
    }
}
