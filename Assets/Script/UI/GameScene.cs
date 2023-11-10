using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    private Transform overlayPanel;
    [SerializeField]
    private Transform winPanel;
    [SerializeField]
    private Transform losePanel;
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Button ingameMenuButton;

    private void Start()
    {
    }

    public void ShowWinPanel()
    {
        overlayPanel.gameObject.SetActive(true);
        winPanel.gameObject.SetActive(true);
        FadeIn(overlayPanel.GetComponent<CanvasGroup>(), winPanel.GetComponent<RectTransform>());
        ingameMenuButton.interactable = false;
        StartCoroutine(SetAchive(winPanel));
    }

    public void ShowLosePanel()
    {
        overlayPanel.gameObject.SetActive(true);
        losePanel.gameObject.SetActive(true);
        FadeIn(overlayPanel.GetComponent<CanvasGroup>(), losePanel.GetComponent<RectTransform>());
        ingameMenuButton.interactable = false;
        StartCoroutine(SetAchive(losePanel));
    }

    private IEnumerator SetAchive(Transform achiveParent)
    {
        Transform achiveContainer = achiveParent.GetChild(2);
        for (int i = 0; i < achiveContainer.childCount; i++)
        {
            yield return new WaitForSecondsRealtime(.3f);
            if (i < GameManager.instance.achivement)
            {
                Transform star = achiveContainer.GetChild(i);
                SetStar(star);
            }
        }
    }

    public void SetTime(float time)
    {
        int minute = (int)time / 60;
        int second = (int)time % 60;
        timeText.text = minute.ToString("00") + ":" + second.ToString("00");
    }

    private void SetStar(Transform star)
    {
        star.localScale = new Vector3(0, 0, 0);
        star.gameObject.SetActive(true);
        star.DOScale(new Vector3(1, 1, 1), .3f).SetEase(Ease.OutBack).SetUpdate(true);
        Transform starDisabler = star.GetChild(0);
        starDisabler.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, RectTransform rectTransform)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0, .3f).SetUpdate(true);

        rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        rectTransform.DOAnchorPos(new Vector2(0, 400), .3f, false).SetEase(Ease.OutQuint).SetUpdate(true);

        yield return new WaitForSecondsRealtime(.3f);

        overlayPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void FadeIn(CanvasGroup canvasGroup, RectTransform rectTransform)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1, .3f).SetUpdate(true);

        rectTransform.anchoredPosition = new Vector3(0, 500, 0);
        rectTransform.DOAnchorPos(new Vector2(0, 0), .3f, false).SetEase(Ease.OutQuint).SetUpdate(true);
    }
}
