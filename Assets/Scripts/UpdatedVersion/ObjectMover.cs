using UnityEngine;
using DG.Tweening;

public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Distance to move from current position")]
    public Vector2 moveOffset = new Vector2(5f, 0f);
    public Ease easeType = Ease.Linear;

    [Tooltip("Movement speed in units per second")]
    public float speed = 2f;



    /// <summary>
    /// Moves the object from its current position to (current position + moveOffset) at the given speed.
    /// </summary>
    public void Move()
    {
        Vector3 targetPosition = transform.position + new Vector3(moveOffset.x, moveOffset.y, 0f);
        float duration = Vector3.Distance(transform.position, targetPosition) / speed;

        transform.DOMove(targetPosition, duration).SetEase(easeType);
    }

    public void MoveAux(float Xoffset)
    {
        Vector3 targetPosition = transform.position + new Vector3(Xoffset, 0f, 0f);
        float duration = Vector3.Distance(transform.position, targetPosition) / speed;

        transform.DOMove(targetPosition, duration).SetEase(easeType);
    }

    // Optional: Call Move automatically for testing
    private void Start()
    {
        // Uncomment to test automatically on Start
        //Move();
    }
}
