using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum ActionWord
{
    right,
    left,
    jump,
    climb,
    interact,
    hide,
    scratch,
    sleep,
    up,
    down,
    jump2,
    awake,
    meow,
    right2,
    ok,
}

[System.Serializable]
public class ActionTypeEvent : UnityEngine.Events.UnityEvent<ActionWord> { }


[Serializable]
public class PathPoint
{
    public bool rotate = true;
    public bool activateDependentDecisions = false;
    public float speed = 3f;
    public AnimationClip animation;
    public Transform point;
    public bool hasEvent;
    public UnityEvent onReachPoint;
}

[Serializable]
public class Decision
{
    public UnityEvent onActivate;
    public bool active = true;
    public ActionPoint nextActionPoint;
    public ActionWord action;
    public DependentDecision[] dependentDecisions;
    public DependentDecision[] deactivateDecisions;

    [ReorderableList]
    public PathPoint[] path;

}

[Serializable]
public class DependentDecision
{
    public ActionPoint actionPoint;
    public ActionWord action;
}

public class ActionPoint : MonoBehaviour
{

    [Header("Action Point")]
    [SerializeField] private Decision[] possibleActions;
    public Decision[] PossibleActions => possibleActions;

    [Header("Gizmos")]
    public float lineWidth = 0.2f;
    public float lineHeight = 0.5f;
    public Color lineColor = Color.red;


    [Header("Gizmos Settings")]
    public bool drawPaths = true;
    [Range(2, 64)] public int labelFontSize = 12;
    [Range(2, 64)] public int actionWordFontSize = 16;
    [Tooltip("Vertical separation between overlapping paths (in world units).")]
    public float pathOffset = 0.25f;
    [Tooltip("Vertical spacing between action labels.")]
    public float labelSpacing = 0.5f;
    [Tooltip("Extra lift for waypoint labels above markers.")]
    public float labelVerticalOffset = 0.12f;


    public void ActivateInteraction(ActionWord action)
    {

        foreach (Decision decision in possibleActions)
        {
            if (!decision.active && decision.action == action)
            {
                decision.active = true;
            }
        }
    }

    public void DeactivateInteraction(ActionWord action)
    {

        foreach (Decision decision in possibleActions)
        {
            if (decision.active && decision.action == action)
            {
                decision.active = false;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = lineColor;
        Gizmos.DrawCube(transform.position + new Vector3(0f, lineHeight, 0f), new Vector3(lineWidth, lineHeight * 2, lineWidth));

        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, new Vector3(lineWidth, lineWidth, lineWidth));

        if (!drawPaths) return;
        if (possibleActions == null || possibleActions.Length == 0) return;

        // Build list of active decisions
        var activeDecisions = new System.Collections.Generic.List<Decision>();
        foreach (var dec in possibleActions)
        {
            if (dec == null) continue;
            if (!dec.active) continue;
            if (dec.path == null || dec.path.Length == 0) continue;
            activeDecisions.Add(dec);
        }

        int n = activeDecisions.Count;
        if (n == 0) return;

        // Draw all decisions
        for (int idx = 0; idx < n; idx++)
        {
            var decision = activeDecisions[idx];
            if (decision == null) continue;

            string actionName = decision.action.ToString();
            Color color = Color.HSVToRGB(
                Mathf.Abs(actionName.GetHashCode() % 1000) / 1000f,
                0.8f, 0.9f);

            Handles.color = color;

            // Lane index: for n=2 -> -0.5, +0.5 (centered separation)
            float laneIndex = idx - (n - 1) * 0.5f;

            // Offset straight in Y axis (2D-friendly)
            Vector3 laneOffset = Vector3.up * (laneIndex * pathOffset);

            // Draw segments: start from ActionPoint position
            Vector3 segStart = transform.position;
            var path = decision.path;

            for (int i = 0; i < path.Length; i++)
            {
                var p = path[i];
                if (p == null || p.point == null) continue;

                Vector3 segEnd = p.point.position;

                // Draw shifted line
                Handles.DrawLine(segStart + laneOffset, segEnd + laneOffset);

                // Sphere marker
                float handleSize = HandleUtility.GetHandleSize(segEnd) * 0.08f;
                Handles.SphereHandleCap(0, segEnd + laneOffset, Quaternion.identity, handleSize, EventType.Repaint);

                // Waypoint label
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = color;
                labelStyle.fontSize = labelFontSize;

                string label = $"Speed: {p.speed}";
                if (p.animation != null) label += $"\nAnim: {p.animation.name}";
                if (p.hasEvent) label += "\nEvent: ✓";

                Vector3 labelPos = segEnd + laneOffset + Vector3.up * (handleSize + labelVerticalOffset);
                Handles.Label(labelPos, label, labelStyle);

                // Advance
                segStart = segEnd;
            }

            // Draw action label stacked above ActionPoint

                GUIStyle actionStyle = new GUIStyle(EditorStyles.boldLabel);
                actionStyle.normal.textColor = color;
                actionStyle.fontSize = actionWordFontSize;

                float baseSize = HandleUtility.GetHandleSize(transform.position) * 0.3f;
                Vector3 actionLabelPos = transform.position + Vector3.up * (baseSize + idx * labelSpacing);

                Handles.Label(actionLabelPos, decision.action.ToString(), actionStyle);
            
        }

        // Root marker for the ActionPoint
        float rootSize = HandleUtility.GetHandleSize(transform.position) * 0.12f;
        Handles.color = Color.white;
        Handles.SphereHandleCap(0, transform.position, Quaternion.identity, rootSize, EventType.Repaint);
    }
#endif
}
