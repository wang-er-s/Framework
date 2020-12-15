using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI点击事件 包含onClick onDown onEnter onExit onUp onSelect onUpdateSelect
/// </summary>
public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate();
    public delegate void VoidDelegate2(PointerEventData eventData);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onDrag;
    public VoidDelegate2 onEndDrag;
    public VoidDelegate2 onScroll;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    
    public override void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke();
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelect?.Invoke();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        onScroll?.Invoke(eventData);
    }

}

