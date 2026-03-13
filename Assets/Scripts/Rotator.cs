using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Degrees to rotate (positive = clockwise depending on axis)")]
    public Vector3 rotationOffset = new Vector3(0f, 0f, 60f);

    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 90f;

    [Header("Tween Settings")]
    [Tooltip("Easing function for rotation")]
    public Ease ease = Ease.Linear;

    public bool autoRotate = false;

    [SerializeField] private UnityEvent onRotate;

    /// <summary>
    /// Rotates the object from its current rotation by rotationOffset at the given speed.
    /// </summary>
    public void Rotate()
    {
        onRotate?.Invoke();
        Vector3 targetRotation = transform.eulerAngles + rotationOffset;
        float duration = rotationOffset.magnitude / rotationSpeed;

        transform.DORotate(targetRotation, duration, RotateMode.FastBeyond360)
                 .SetEase(ease);    
    }

    // Optional: Call Rotate automatically for testing
    private void Start()
    {
        // Uncomment to test automatically on Start

        if (autoRotate)
            Rotate();
    }
}
