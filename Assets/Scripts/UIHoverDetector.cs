using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit.Invoke();
    }
}