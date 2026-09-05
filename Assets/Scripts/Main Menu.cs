using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    public Toggle musicToggle;
    public Toggle sfxToggle;

    public static bool isMusicOn = true;
    public static bool isSFXOn = true;

    public GameObject scoresPanel;
    public TMPro.TextMeshProUGUI scoreText1;
    public TMPro.TextMeshProUGUI scoreText2;
    public TMPro.TextMeshProUGUI scoreText3;

    public void PlayGame()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        PlayerController.ResetPlayerInstance();

        RoomSpawner.totalRoomsVisited = 1;
        SceneManager.LoadScene("Room_1");
    }

    public void OpenSettings()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        if (musicToggle != null) musicToggle.isOn = isMusicOn;
        if (sfxToggle != null) sfxToggle.isOn = isSFXOn;
    }

    public void OpenControls()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ToggleMusic(bool isOn)
    {
        isMusicOn = isOn;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.UpdateMusicVolume();
        }
    }

    public void ToggleSFX(bool isOn)
    {
        isSFXOn = isOn;
    }


    public void ShowScores()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (scoresPanel != null) scoresPanel.SetActive(true);

        System.Collections.Generic.List<ScoreManager.ScoreEntry> topScores = ScoreManager.GetTopScores();

        if (scoreText1 != null && topScores.Count > 0)
            scoreText1.text = "1. " + topScores[0].initials + "  " + topScores[0].score.ToString("D5");

        if (scoreText2 != null && topScores.Count > 1)
            scoreText2.text = "2. " + topScores[1].initials + "  " + topScores[1].score.ToString("D5");

        if (scoreText3 != null && topScores.Count > 2)
            scoreText3.text = "3. " + topScores[2].initials + "  " + topScores[2].score.ToString("D5");
    }

    public void CloseScores()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlayBtnClick();

        if (scoresPanel != null) scoresPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
