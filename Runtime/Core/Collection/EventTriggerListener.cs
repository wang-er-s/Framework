using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI点击事件 包含onClick onDown onEnter onExit onUp onSelect onUpdateSelect
/// </summary>
public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate2(PointerEventData eventData);
    public event Action onClick;
    public event Action onDown;
    public event Action onDrag;
    public event Action<PointerEventData> onEndDrag;
    public event Action<PointerEventData> onScroll;
    public event Action onEnter;
    public event Action onExit;
    public event Action onUp;
    public event Action onSelect;
    public event Action onUpdateSelect;

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

