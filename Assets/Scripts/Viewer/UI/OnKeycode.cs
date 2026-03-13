using UnityEngine;
using UnityEngine.Events;

public class OnKeycode : MonoBehaviour
{
    [SerializeField] private KeyCode _keycode;
    public UnityEvent _unityEvent;
    
    private void Update()
    {
        if (Input.GetKeyDown(_keycode))
        {
            _unityEvent?.Invoke();
        }
    }
}
