using System;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button , IComponentEvent ,IFieldChangeCb<bool>
{
    [SerializeField]
    private float singleClickIntervalTime = 0.3f;
    [SerializeField]
    private float doubleClickIntervalTime = 0.3f;
    [SerializeField] 
    private float longClickTime = 1;
    private float _lastUpTime;
    private float _lastDownTime;
    private float _downTime;
    private float _upTime;

    public ButtonClickedEvent OnSingleClick { get; } = new ButtonClickedEvent(); 
    public ButtonClickedEvent OnDoubleClick { get; } = new ButtonClickedEvent(); 
    public UnityEvent OnLongClick { get; } = new UnityEvent();
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if(!IsInteractable()) return;
        _lastDownTime = _downTime;
        _downTime = Time.time;
    }
    
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if(!IsInteractable()) return;
        var time = Time.time;
        _lastUpTime = _upTime;
        _upTime = time;
        CheckDoubleClick();
        CheckSingleClick();
        CheckLongClick();
    }

    private bool CheckLongClick()
    {
        var pressTime = _upTime - _downTime; 
        if (pressTime < longClickTime) return false;
        OnLongClick.Invoke();
        return true;
    }

    private bool CheckSingleClick()
    {
        if(_upTime - _lastUpTime < singleClickIntervalTime) return false;
        onClick= OnSingleClick;
        return true;
    }

    private int _clickCount;
    private bool CheckDoubleClick()
    {
        if (_upTime - _lastUpTime > doubleClickIntervalTime)
        {
            _clickCount = 1;
            return false;
        }
        _clickCount++;
        if (_clickCount >= 2)
        {
            OnDoubleClick.Invoke();
            _clickCount = 0;
            return true;
        }
        return false;
    }

    public UnityEvent GetComponentEvent()
    {
        return OnSingleClick;
    }

    public Action<bool> GetFieldChangeCb()
    {
        return value => gameObject.SetActive(value);
    }
}