using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    private static GameHUD instance;
    
    // Left HUD
    public Image healthBarFill;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI grenadesText;

    public TextMeshProUGUI scoreText;

    public static int globalScore = 0;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(transform.root.gameObject);
            return;
        }
    }

    void Update()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        PlayerController player = playerObj.GetComponent<PlayerController>();
        if (player == null) return;

        if (healthBarFill != null)
        {
            float healthPercentage = (float)Mathf.Max(0, player.currentHealth) / player.maxHealth;
            healthBarFill.fillAmount = healthPercentage;
        }

        if (ammoText != null)
        {
            ammoText.text = player.ammoCount + "/" + player.reserveAmmo;
        }

        if (grenadesText != null)
        {
            grenadesText.text = player.grenadeCount.ToString();
        }

        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + globalScore.ToString("D5");
        }
    }

    public static void AddPoints(int points)
    {
        globalScore += points;
    }
}
