using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private float Health;
    private float MaxHealth;
    public float Width;
    public float Height;

    [SerializeField]
    private RectTransform healthBar;

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }

    public void SetHealth(float health)
    {
        Health = health;

        float healthPercentage = Mathf.Clamp01(Health / MaxHealth);

        float newHeight = healthPercentage * Height;

        healthBar.sizeDelta = new Vector2(Width, newHeight);
    }
}
