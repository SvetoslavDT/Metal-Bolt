using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public enum BonusType { None, Ammo, Health, Grenades }

    public Sprite closedSprite;
    public Sprite openSprite;

    public BonusType bonusType = BonusType.Ammo;

    public string nextSceneName;

    public bool startAsAlwaysClosed = false;

    private bool isOpen = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public Transform targetPlayerSpawnPoint;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }

        if (startAsAlwaysClosed)
        {
            isOpen = false;
            if (boxCollider != null) boxCollider.isTrigger = false;
        }
    }

    public void OpenDoor()
    {
        if (startAsAlwaysClosed) return;

        isOpen = true;
        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        if (boxCollider != null) boxCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            GiveBonus(other.gameObject);

            RoomSpawner.totalRoomsVisited++;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayDoorPass();
            }

            if (targetPlayerSpawnPoint != null)
            {
                other.transform.position = targetPlayerSpawnPoint.position;

                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }


    void GiveBonus(GameObject playerObj)
    {
        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player == null) return;

        switch (bonusType)
        {
            case BonusType.Ammo:
                player.reserveAmmo += 30;
                break;
            case BonusType.Health:
                player.currentHealth = Mathf.Min(player.currentHealth + 50, player.maxHealth);
                if (player.GetComponent<Animator>() != null)
                {
                    player.GetComponent<Animator>().SetInteger("health", player.currentHealth);
                }
                HealthBarUI uiBar = Object.FindAnyObjectByType<HealthBarUI>();
                if (uiBar != null)
                {
                    uiBar.SetHealth(player.currentHealth);
                }
                break;
            case BonusType.Grenades:
                player.grenadeCount += 1;
                break;
        }
    }
}
