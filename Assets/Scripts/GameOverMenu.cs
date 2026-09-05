using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public TMP_InputField initialsInputField;

    private int finalScore;
    private bool isGameOverActive = false;
    private bool hasNewHighScore = false;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (isGameOverActive) return;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        isGameOverActive = true;

        yield return new WaitForSeconds(1.5f);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        finalScore = GameHUD.globalScore;
        hasNewHighScore = ScoreManager.IsNewHighScore(finalScore);

        if (hasNewHighScore)
        {
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = "NEW HIGH SCORE: " + finalScore.ToString("D5");
                gameOverScoreText.color = Color.yellow;
            }

            if (initialsInputField != null)
            {
                initialsInputField.gameObject.SetActive(true);
                initialsInputField.text = "";

                yield return new WaitForEndOfFrame();

                Debug.Log("InputField exists: " + initialsInputField.name);
                Debug.Log("EventSystem exists: " + (UnityEngine.EventSystems.EventSystem.current != null));

                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(initialsInputField.gameObject);

                initialsInputField.Select();
                initialsInputField.ActivateInputField();
            }
        }
        else
        {
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = "FINAL SCORE: " + finalScore.ToString("D5");
                gameOverScoreText.color = Color.white;
            }

            if (initialsInputField != null)
            {
                initialsInputField.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!isGameOverActive || gameOverPanel == null || !gameOverPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (hasNewHighScore)
            {
                string text = initialsInputField != null ? initialsInputField.text.Trim() : "";

                if (text.Length >= 1 && text.Length <= 3)
                {
                    ProcessAndExit();
                }
                else
                {
                    if (initialsInputField != null) initialsInputField.ActivateInputField();
                }
            }
            else
            {
                ProcessAndExit();
            }
        }
    }

    void ProcessAndExit()
    {
        if (hasNewHighScore)
        {
            string enteredInitials = initialsInputField != null ? initialsInputField.text : "AAA";
            ScoreManager.SaveHighScore(enteredInitials, finalScore);
        }

        PlayerController activePlayer = Object.FindAnyObjectByType<PlayerController>();
        if (activePlayer != null)
        {
            Destroy(activePlayer.gameObject);
        }

        GameHUD.globalScore = 0;

        if (transform.root != null)
        {
            Destroy(transform.root.gameObject);
        }

        SceneManager.LoadScene("Main Menu");
    }

}
