using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class CopyPasteTransform
{
    private static Vector3 copiedPosition;
    private static Quaternion copiedRotation;
    private static Vector3 copiedScale;
    private static bool hasCopied = false;

    // Copy: Ctrl+Shift+C (Windows) or Cmd+Shift+C (Mac)
    [MenuItem("Edit/Copy World Transform %&c")]
    private static void CopyWorldTransform()
    {
        if (Selection.activeTransform == null) return;

        Transform t = Selection.activeTransform;
        copiedPosition = t.position;
        copiedRotation = t.rotation;
        copiedScale = t.lossyScale;
        hasCopied = true;

        Debug.Log($"Copied Transform of {t.name}");
    }

    // Paste: Ctrl+Shift+V (Windows) or Cmd+Shift+V (Mac)
    [MenuItem("Edit/Paste World Transform %&v")]
    private static void PasteWorldTransform()
    {
        if (!hasCopied) return;
        if (Selection.activeTransform == null) return;

        Undo.RecordObjects(Selection.transforms, "Paste World Transform");

        foreach (Transform t in Selection.transforms)
        {
            // To apply lossy scale, we need to adjust localScale relative to parent
            t.position = copiedPosition;
            t.rotation = copiedRotation;

            if (t.parent != null)
                t.localScale = Vector3.Scale(copiedScale, InverseLossyScale(t.parent));
            else
                t.localScale = copiedScale;
        }

        Debug.Log("Pasted Transform");
    }

    // Helper: Inverse lossy scale
    private static Vector3 InverseLossyScale(Transform t)
    {
        Vector3 s = t.lossyScale;
        return new Vector3(
            s.x != 0 ? 1f / s.x : 0f,
            s.y != 0 ? 1f / s.y : 0f,
            s.z != 0 ? 1f / s.z : 0f
        );
    }
}
