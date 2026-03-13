using UnityEngine;
using DG.Tweening;

public class Hover2D : MonoBehaviour
{
    [SerializeField] private float hoverAmount = 0.5f;   // How high/low it moves
    [SerializeField] private float duration = 1f;        // Time to go up or down
    [SerializeField] private Ease easeType = Ease.InOutSine; // Exposed in inspector
    [SerializeField] private bool startImmediately = true;

    private Vector3 startPosition;
    private Tweener hoverTweener;

    [SerializeField] private bool axisX = false; // New option for X axis hover
    void Start()
    {
        startPosition = transform.position;

        if (startImmediately)
            StartHover();
    }

    public void StartHover()
    {
        // Kill any existing tween to avoid duplicates
        hoverTweener?.Kill();

        if(axisX)
        {
            // Tween left and right infinitely
            hoverTweener = transform.DOMoveX(startPosition.x + hoverAmount, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(easeType); // Use inspector-selected ease
            return;
        }

        // Tween up and down infinitely
        hoverTweener = transform.DOMoveY(startPosition.y + hoverAmount, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(easeType); // Use inspector-selected ease
    }

    public void StopHover()
    {
        hoverTweener?.Kill();
        transform.position = startPosition;
    }
}
