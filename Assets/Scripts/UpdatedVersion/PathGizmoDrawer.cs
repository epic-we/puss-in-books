using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class PathGizmoDrawer : MonoBehaviour
{
    [SerializeField] private Color lineColor = Color.yellow;
    [SerializeField] private Color pointColor = Color.red;
    [SerializeField] private float pointRadius = 0.2f;

    private void OnDrawGizmos()
    {
        int totalPoints = transform.childCount + 1; // include self
        if (totalPoints < 2) return;

        Gizmos.color = pointColor;

        // Start with the parent (self)
        Transform prev = transform;

        // Draw sphere at self
        Gizmos.DrawWireSphere(prev.position, pointRadius);

        // Loop through children
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            // Draw sphere at child
            Gizmos.color = pointColor;
            Gizmos.DrawWireSphere(child.position, pointRadius);

            // Draw line from previous to current
            Gizmos.color = lineColor;
            Gizmos.DrawLine(prev.position, child.position);

            prev = child;
        }
    }
}
#endif
