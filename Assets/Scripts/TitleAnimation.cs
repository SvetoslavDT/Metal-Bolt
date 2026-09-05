using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    public RectTransform titleTextTransform;
    public float targetYPosition = 300f;
    public float moveSpeed = 400f;

    public GameObject ghostTextPrefab;
    public float ghostSpawnInterval = 0.05f;
    public float ghostFadeSpeed = 2f;

    public GameObject mainMenuPanel;

    private bool isMoving = true;

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        if (titleTextTransform != null)
        {
            titleTextTransform.anchoredPosition = new Vector2(titleTextTransform.anchoredPosition.x, -Screen.height / 2f - 100f);
        }

        StartCoroutine(SpawnGhostTrail());

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMenuMusic();
        }
    }

    void Update()
    {
        if (!isMoving || titleTextTransform == null) return;

        Vector2 currentPos = titleTextTransform.anchoredPosition;
        currentPos.y += moveSpeed * Time.deltaTime;
        titleTextTransform.anchoredPosition = currentPos;

        if (currentPos.y >= targetYPosition)
        {
            currentPos.y = targetYPosition;
            titleTextTransform.anchoredPosition = currentPos;
            isMoving = false;

            ShowMenuButtons();
        }
    }

    IEnumerator SpawnGhostTrail()
    {
        while (isMoving)
        {
            if (titleTextTransform != null && ghostTextPrefab != null)
            {
                GameObject ghost = Instantiate(ghostTextPrefab, titleTextTransform.parent);
                RectTransform ghostRt = ghost.GetComponent<RectTransform>();
                ghostRt.anchoredPosition = titleTextTransform.anchoredPosition;
                ghostRt.localScale = titleTextTransform.localScale;

                ghost.transform.SetSiblingIndex(1);

                StartCoroutine(FadeOutGhost(ghost));
            }
            yield return new WaitForSeconds(ghostSpawnInterval);
        }
    }

    IEnumerator FadeOutGhost(GameObject ghostObj)
    {
        CanvasGroup canvasGroup = ghostObj.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = ghostObj.AddComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            float alpha = 0.6f;
            canvasGroup.alpha = alpha;

            while (alpha > 0f)
            {
                alpha -= ghostFadeSpeed * Time.deltaTime;
                canvasGroup.alpha = alpha;
                yield return null;
            }
        }

        Destroy(ghostObj);
    }

    void ShowMenuButtons()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }
}
