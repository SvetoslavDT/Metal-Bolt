using UnityEngine;

public class PlayerGrenades : MonoBehaviour
{
    public GameObject explosionPrefab;
    public int grenadeCount = 3;
    public float distanceForward = 2f;

    private PlayerController playerController;
    private Animator animator;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    public void TryThrowGrenade(Vector2 facingDirection)
    {
        if (grenadeCount > 0)
        {
            grenadeCount--;
            Debug.Log("Grenade thrown! Left: " + grenadeCount);

            if (animator != null)
            {
                animator.SetTrigger("throwGrenade");
            }

            Vector3 explosionPosition = transform.position + (Vector3)(facingDirection * distanceForward);

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            }
        }
    }

}
