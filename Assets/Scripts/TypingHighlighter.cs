using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TypingHighlighter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textMeshPro;

    [Header("Target Settings")]
    [SerializeField] private string targetWord;
    [SerializeField] private string targetScene;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Ghostly Effect")]
    public float floatAmplitude = 2f;
    public float floatFrequency = 2f;
    [Range(0f, 1f)] public float ghostAlpha = 0.5f;

    [Header("Cursor Blink")]
    public float cursorBlinkSpeed = 2f;

    private LevelPicker levelPicker;
    private int currentIndex = 0;

    private Vector3[][] originalVertices;
    private bool wordCompleted = false;

    // cursor data
    private bool showCursor = true;
    private float blinkTimer = 0f;

    void Awake()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TMP_Text>();
    }

    public void Initialize(string word, string sceneName, LevelPicker picker)
    {
        targetWord = word;
        targetScene = sceneName;
        levelPicker = picker;
        RefreshText();
        CacheOriginalVertices();
    }

    void Update()
    {
        HandleInput();
        AnimateUntypedLetters();
        BlinkCursor();
    }

    private void HandleInput()
    {
        if (wordCompleted) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    RefreshText();
                }
            }
            else
            {
                while (currentIndex < targetWord.Length && targetWord[currentIndex] == ' ')
                    currentIndex++;

                if (currentIndex < targetWord.Length &&
                    char.ToUpper(c) == char.ToUpper(targetWord[currentIndex]))
                {
                    currentIndex++;
                    RefreshText();
                }
            }
        }
    }

    private void RefreshText()
    {
        if (string.IsNullOrEmpty(targetWord)) return;

        string hexHighlight = ColorUtility.ToHtmlStringRGB(highlightColor);
        string highlighted = "";
        string remaining = "";

        if (currentIndex > 0)
            highlighted = $"<color=#{hexHighlight}>{targetWord.Substring(0, currentIndex)}</color>";

        if (currentIndex < targetWord.Length)
        {
            Color ghostColor = textMeshPro.color;
            ghostColor.a = ghostAlpha;
            string ghostHex = ColorUtility.ToHtmlStringRGBA(ghostColor);
            remaining = $"<color=#{ghostHex}>{targetWord.Substring(currentIndex)}</color>";
        }

        // Add a plain | (cursor placeholder) at the end — we’ll blink it manually
        textMeshPro.text = highlighted + "|" + remaining;

        CacheOriginalVertices();

        if (currentIndex >= targetWord.Length)
        {
            wordCompleted = true;
            OnWordCompleted();
        }
    }

    private void CacheOriginalVertices()
    {
        textMeshPro.ForceMeshUpdate(true, true);
        var info = textMeshPro.textInfo;
        if (info == null || info.meshInfo == null) return;

        originalVertices = new Vector3[info.meshInfo.Length][];
        for (int i = 0; i < info.meshInfo.Length; i++)
        {
            int len = info.meshInfo[i].vertices.Length;
            originalVertices[i] = new Vector3[len];
            System.Array.Copy(info.meshInfo[i].vertices, originalVertices[i], len);
        }
    }

    private void AnimateUntypedLetters()
    {
        if (textMeshPro == null || originalVertices == null) return;
        var info = textMeshPro.textInfo;
        if (info.characterCount == 0) return;

        float time = Time.unscaledTime * floatFrequency;

        for (int i = 0; i < info.characterCount; i++)
        {
            if (i < currentIndex) continue; // typed letters stay still

            var charInfo = info.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            var verts = info.meshInfo[meshIndex].vertices;
            var origVerts = originalVertices[meshIndex];
            if (origVerts == null || vertexIndex + 3 >= origVerts.Length) continue;

            float offset = Mathf.Sin(time + charInfo.vertex_BL.position.x * 0.05f) * floatAmplitude;
            Vector3 move = new Vector3(0, offset, 0);

            verts[vertexIndex + 0] = origVerts[vertexIndex + 0] + move;
            verts[vertexIndex + 1] = origVerts[vertexIndex + 1] + move;
            verts[vertexIndex + 2] = origVerts[vertexIndex + 2] + move;
            verts[vertexIndex + 3] = origVerts[vertexIndex + 3] + move;
        }

        // apply all mesh changes
        for (int i = 0; i < info.meshInfo.Length; i++)
        {
            var meshInfo = info.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMeshPro.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void BlinkCursor()
    {
        if (wordCompleted) return;

        blinkTimer += Time.unscaledDeltaTime * cursorBlinkSpeed;
        if (blinkTimer >= 1f)
        {
            showCursor = !showCursor;
            blinkTimer = 0f;
        }

        var info = textMeshPro.textInfo;
        if (info.characterCount == 0) return;

        // Find the cursor character (it’s right after the last typed one)
        int cursorIndex = currentIndex;
        if (cursorIndex < info.characterCount)
        {
            var charInfo = info.characterInfo[cursorIndex];
            if (charInfo.isVisible)
            {
                int meshIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                var colors = info.meshInfo[meshIndex].colors32;
                Color32 color = showCursor
                    ? (Color32)highlightColor
                    : new Color32((byte)highlightColor.r, (byte)highlightColor.g, (byte)highlightColor.b, 0);

                colors[vertexIndex + 0] = color;
                colors[vertexIndex + 1] = color;
                colors[vertexIndex + 2] = color;
                colors[vertexIndex + 3] = color;

                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }
    }

    private void OnWordCompleted()
    {
        textMeshPro.text =
            $"<color=#{ColorUtility.ToHtmlStringRGB(highlightColor)}>{targetWord}</color>";
        levelPicker?.AnimateDown(targetScene);
    }
}
