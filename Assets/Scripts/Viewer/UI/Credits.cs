using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Credits : ScrollRect
{
    [SerializeField] private CreditsInspector _inspector;

    protected override void Awake()
    {
        base.Awake();

        _inspector = GetComponent<CreditsInspector>();
        if ( _inspector == null )
        {
            Debug.Log("Credits inspector not found, turning off. ");
            enabled = false;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if ( !_inspector.AllowSkip ) return;
        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if ( !_inspector.AllowSkip ) return;
        base.OnDrag(eventData);

        _inspector.SetDrag(true);
        
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if ( !_inspector.AllowSkip ) return;
        base.OnEndDrag(eventData);

        _inspector.SetDrag(false);
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if ( !_inspector.AllowSkip ) return;
        base.OnInitializePotentialDrag(eventData);
    }
    public override void OnScroll(PointerEventData eventData)
    {
        if ( !_inspector.AllowSkip ) return;
        base.OnScroll(eventData);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inspector.SetDrag(false);
    }
}
