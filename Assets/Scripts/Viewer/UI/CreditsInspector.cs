using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsInspector : MonoBehaviour
{
    [field:SerializeField] public bool AllowSkip { get; private set; } = true;
    // serialized cuz i was debugging
    [field: SerializeField] public bool LetDrag { get; private set; } = false;
    [SerializeField] private  CanvasGroup _background;
    [SerializeField] [Range(0f,1f)] private float _minAlpha = 0f;
    [SerializeField] [Range(0f,1f)] private float _maxAlpha = 0.75f;
    [SerializeField] private  bool _goBack = false;
    [SerializeField] private float _scrollTime = 30f;
    [SerializeField] private float _gameLogoTime = 3f;
    [SerializeField] private Credits _credits;
    [SerializeField] private CanvasGroup _content;
    [SerializeField] private CanvasGroup _gameLogo;
    [SerializeField] private GameObject _gameObject;

    private Coroutine _cor;
    private Coroutine _backCor;

    private void OnEnable()
    {
        _loop = null;
        if (_gameLogo != null)
            _gameLogo.alpha = 0f;
        _background.alpha = 0f;
        _content.alpha = 0f;
        _credits.verticalNormalizedPosition = 1f;

        _backCor = StartCoroutine(Fade(_background, true, _minAlpha, _maxAlpha, () => _cor = null));
        StartCoroutine(Fade(_content, true, _minAlpha, _maxAlpha, () => _cor = null));
    }

    public void StartWithFullBackground()
    {
        if (_backCor != null)
            StopCoroutine(_backCor);
        _background.alpha = 1f;
    }

    private void Update()
    {
        if (LetDrag) return;

        if (_credits.verticalNormalizedPosition > 0f)
            _credits.verticalNormalizedPosition -= Mathf.Max(0f, Time.deltaTime / _scrollTime);
        else if (_loop == null)
        {
            _loop = StartCoroutine(Logo());
        }
    }

    private Coroutine _loop;

    private IEnumerator Logo()
    {

        yield return new WaitForSeconds(_gameLogoTime / 3f*2f);
        yield return Fade(_content, false, _minAlpha, _maxAlpha, () => _cor = null);
        yield return new WaitForSeconds(_gameLogoTime / 2f);

        if (_gameLogo != null)
        {
        yield return Fade(_gameLogo, true, _minAlpha, _maxAlpha, () => _cor = null);
        yield return new WaitForSeconds(_gameLogoTime);
        yield return Fade(_gameLogo, false, _minAlpha, _maxAlpha, () => _cor = null);

        yield return new WaitForSeconds(_gameLogoTime / 4f);
        }

        yield return Fade(_background, false, _minAlpha, _maxAlpha, () => _cor = null);

        _gameObject.SetActive(false);
        if (_goBack)
            SceneManager.LoadScene("NewMainMenu");
    }

    public static IEnumerator Fade(CanvasGroup cg, bool inOrOut, float _minAlpha, float _maxAlpha, Action onComplete = null)
    {
        float initAlpha = _minAlpha;
        float finalAlpha = _maxAlpha;
        float current = initAlpha;

        Debug.Log("in: " + initAlpha + " out: " + finalAlpha + " delta: " + Time.deltaTime / 0.8f * Mathf.Sign(finalAlpha - initAlpha));

        float newAlpha;

        do
        {
            current += Time.deltaTime / 0.8f;
            newAlpha = Mathf.SmoothStep(initAlpha, finalAlpha, current);
            if (!inOrOut)
                newAlpha = _maxAlpha - newAlpha;
            cg.alpha = newAlpha;

            yield return null;
        }
        while (newAlpha < _maxAlpha && newAlpha > _minAlpha);

        if (!inOrOut)
            cg.alpha = initAlpha;
        else
            cg.alpha = finalAlpha;
        
        onComplete?.Invoke();
    }

    public void SetDrag(bool dragging)
    {
        Debug.Log("Setting drag. ");
        LetDrag = dragging;
    }
}
