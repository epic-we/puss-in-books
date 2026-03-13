using UnityEngine;

public class ForceAspect : MonoBehaviour
{
    private Camera cam;
    private const float targetAspect = 16f / 9f;

    void Start()
    {
        cam = Camera.main;
        ApplyAspect();
    }

    void OnValidate()   // Updates in editor
    {
        ApplyAspect();
    }

    void Update()       // Handles window resize (PC builds)
    {
        ApplyAspect();
    }

    void ApplyAspect()
    {
        if (!cam) return;

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterbox (top + bottom)
            float inset = (1.0f - scaleHeight) / 2.0f;
            cam.rect = new Rect(0f, inset, 1f, scaleHeight);
        }
        else
        {
            // Pillarbox (left + right)
            float scaleWidth = 1.0f / scaleHeight;
            float inset = (1.0f - scaleWidth) / 2.0f;
            cam.rect = new Rect(inset, 0f, scaleWidth, 1f);
        }
    }
}
