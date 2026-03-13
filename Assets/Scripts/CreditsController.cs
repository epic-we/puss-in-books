using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CreditsController : MonoBehaviour
{
    [Header("Credits Movement")]
    [SerializeField] private RectTransform creditsPivot;
    [SerializeField] private RectTransform startPosition;
    [SerializeField] private RectTransform endPosition;
    [SerializeField] private float scrollSpeed = 200f; // pixels per second

    [Header("Background")]
    [SerializeField] private Image creditsBackground;
    [SerializeField] private Material scrollMaterial;

    [Header("Credits Content")]
    [SerializeField] private TextMeshProUGUI[] creditsText;
    [SerializeField] private Image[] creditsImages;

    [Header("Outro Images")]
    [SerializeField] private Image[] outroImages;

    [Header("Timing")]
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float endHoldDuration = 3f;
    [SerializeField] private float outroHoldDuration = 1.5f;

    private Sequence creditsSequence;

    void Start()
    {
        PlayCredits();
    }

    private void PlayCredits()
    {
        ResetVisuals();

        float distanceY = Mathf.Abs(
            endPosition.anchoredPosition.y - startPosition.anchoredPosition.y
        );

        float scrollDuration = distanceY / scrollSpeed;

        creditsSequence = DOTween.Sequence();

        creditsSequence
            // Fade background in
            .Append(creditsBackground.DOFade(1f, fadeDuration))

            .AppendInterval(startDelay)

            // Apply scrolling material
            .AppendCallback(() =>
            {
                creditsBackground.material = scrollMaterial;
            })

            // Scroll credits (Y only)
            .Append(
                creditsPivot
                    .DOAnchorPosY(endPosition.anchoredPosition.y, scrollDuration)
                    .SetEase(Ease.Linear)
            )

            // Hold at end
            .AppendInterval(endHoldDuration)

            // Fade OUT credits content
            .Append(FadeCreditsContent(0f))

            // Fade IN outro images
            .Append(FadeOutroImages(1f))

            .AppendInterval(outroHoldDuration)

            // Fade OUT outro images
            .Append(FadeOutroImages(0f))

            // Remove material
            .AppendCallback(() =>
            {
                creditsBackground.material = null;
            })

            // Fade background out
            .Append(creditsBackground.DOFade(0f, fadeDuration));
    }

    // =========================
    // Helpers
    // =========================

    private void ResetVisuals()
    {
        creditsPivot.anchoredPosition = startPosition.anchoredPosition;
        creditsBackground.material = null;

        SetBackgroundAlpha(0f);
        SetCreditsAlpha(1f);
        SetOutroAlpha(0f);
    }

    private Tween FadeCreditsContent(float targetAlpha)
    {
        Sequence seq = DOTween.Sequence();

        foreach (var text in creditsText)
            seq.Join(text.DOFade(targetAlpha, fadeDuration));

        foreach (var img in creditsImages)
            seq.Join(img.DOFade(targetAlpha, fadeDuration));

        return seq;
    }

    private Tween FadeOutroImages(float targetAlpha)
    {
        Sequence seq = DOTween.Sequence();

        foreach (var img in outroImages)
            seq.Join(img.DOFade(targetAlpha, fadeDuration));

        return seq;
    }

    private void SetCreditsAlpha(float alpha)
    {
        foreach (var text in creditsText)
            SetTMPAlpha(text, alpha);

        foreach (var img in creditsImages)
            SetImageAlpha(img, alpha);
    }

    private void SetOutroAlpha(float alpha)
    {
        foreach (var img in outroImages)
            SetImageAlpha(img, alpha);
    }

    private void SetBackgroundAlpha(float alpha)
    {
        SetImageAlpha(creditsBackground, alpha);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void SetTMPAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    private void OnDestroy()
    {
        creditsSequence?.Kill();
    }
}
