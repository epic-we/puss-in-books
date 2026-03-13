using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GhostlyTextUI : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;

    [Header("Ghostly Motion")]
    [Tooltip("How far each letter floats up and down.")]
    public float amplitude = 2f;
    [Tooltip("How fast the floating oscillates.")]
    public float frequency = 2f;
    [Tooltip("How quickly the wave travels horizontally.")]
    public float waveSpeed = 1f;

    [Header("Ghostly Look")]
    [Range(0f, 1f)]
    [Tooltip("Overall transparency of the text.")]
    public float alpha = 0.5f;

    private string lastText = "";

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        ForceCacheVertices();
    }

    void Update()
    {
        if (tmp == null || !tmp.IsActive())
            return;

        // If text changed, recache vertices
        if (tmp.text != lastText)
        {
            ForceCacheVertices();
            lastText = tmp.text;
        }

        tmp.ForceMeshUpdate();
        textInfo = tmp.textInfo;

        if (textInfo == null || textInfo.characterCount == 0)
            return;

        float time = Time.unscaledTime * waveSpeed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            if (meshIndex >= textInfo.meshInfo.Length || originalVertices == null)
                continue;

            var vertices = textInfo.meshInfo[meshIndex].vertices;
            var origVerts = originalVertices[meshIndex];
            if (origVerts == null || vertexIndex + 3 >= origVerts.Length)
                continue;

            float offset = Mathf.Sin(time + charInfo.vertex_BL.position.x * 0.05f) * amplitude;
            Vector3 move = new Vector3(0, offset, 0);

            // Apply offset relative to cached original vertices
            vertices[vertexIndex + 0] = origVerts[vertexIndex + 0] + move;
            vertices[vertexIndex + 1] = origVerts[vertexIndex + 1] + move;
            vertices[vertexIndex + 2] = origVerts[vertexIndex + 2] + move;
            vertices[vertexIndex + 3] = origVerts[vertexIndex + 3] + move;
        }

        // Apply modified vertices to mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }

        // Fade effect
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    private void ForceCacheVertices()
    {
        tmp.ForceMeshUpdate(true, true);
        textInfo = tmp.textInfo;

        if (textInfo == null || textInfo.meshInfo == null)
            return;

        originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            int len = textInfo.meshInfo[i].vertices.Length;
            originalVertices[i] = new Vector3[len];
            System.Array.Copy(textInfo.meshInfo[i].vertices, originalVertices[i], len);
        }

        lastText = tmp.text;
    }
}
