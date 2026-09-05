using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string initials;
        public int score;

        public ScoreEntry(string initials, int score)
        {
            this.initials = initials;
            this.score = score;
        }
    }

    public static List<ScoreEntry> GetTopScores()
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();

        for (int i = 0; i < 3; i++)
        {
            string initials = PlayerPrefs.GetString("HighScoreInitials_" + i, "---");
            int score = PlayerPrefs.GetInt("HighScoreValue_" + i, 0);

            scores.Add(new ScoreEntry(initials, score));
        }

        return scores;
    }

    public static bool IsNewHighScore(int currentScore)
    {
        if (currentScore <= 0) return false;

        List<ScoreEntry> topScores = GetTopScores();

        foreach (ScoreEntry entry in topScores)
        {
            if (currentScore > entry.score || entry.score == 0)
            {
                return true;
            }
        }
        return false;
    }

    public static void SaveHighScore(string initials, int currentScore)
    {
        if (string.IsNullOrEmpty(initials) || initials.Trim() == "")
        {
            initials = "AAA";
        }

        initials = initials.ToUpper();
        if (initials.Length > 3) initials = initials.Substring(0, 3);

        List<ScoreEntry> scores = GetTopScores();

        scores.Add(new ScoreEntry(initials, currentScore));

        scores.Sort((x, y) => y.score.CompareTo(x.score));

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetString("HighScoreInitials_" + i, scores[i].initials);
            PlayerPrefs.SetInt("HighScoreValue_" + i, scores[i].score);
        }

        PlayerPrefs.Save();
        Debug.Log("Record saved!");
    }

    [ContextMenu("Clear High Scores")]
    public void ClearScores()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All hogh scores deleted!");
    }
}
