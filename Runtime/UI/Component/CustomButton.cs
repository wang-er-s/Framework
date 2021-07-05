using System;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.UI.Wrap.Base;
using TMPro;
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
    private float lastUpTime;
    private float lastDownTime;
    private float downTime;
    private float upTime;

    private static Material grayMat;
    private Material selfMat;
    private CanvasRenderer canvasRenderer;
    private Text textComponent;
    private TextMeshProUGUI tmp;
    public string Text
    {
        get
        {
            if (textComponent != null) return textComponent.text;
            if (tmp != null) return tmp.text;
            return String.Empty;
        }
        set
        {
            if (textComponent != null) textComponent.text = value;
            if (tmp != null) tmp.text = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        canvasRenderer = GetComponent<CanvasRenderer>();
        textComponent = GetComponentInChildren<Text>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        selfMat = targetGraphic.material;
    }

    protected override async void Start()
    {
        base.Start();
        if (grayMat == null && Application.isPlaying)
            grayMat = Resources.Load<Material>("ImageGrayMaterial");
    }

    public ButtonClickedEvent OnSingleClick { get; } = new ButtonClickedEvent(); 
    public ButtonClickedEvent OnDoubleClick { get; } = new ButtonClickedEvent(); 
    public UnityEvent OnLongClick { get; } = new UnityEvent();
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if(!IsInteractable()) return;
        lastDownTime = downTime;
        downTime = Time.time;
    }

    public void SetGray(bool interactable = true)
    {
        targetGraphic.material = grayMat;
        this.interactable = interactable;
    }

    public void SetNormal()
    {
        targetGraphic.material = selfMat;
        interactable = true;
    }
    
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if(!IsInteractable()) return;
        var time = Time.time;
        lastUpTime = upTime;
        upTime = time;
        CheckDoubleClick();
        CheckSingleClick();
        CheckLongClick();
    }

    private bool CheckLongClick()
    {
        var pressTime = upTime - downTime; 
        if (pressTime < longClickTime) return false;
        OnLongClick.Invoke();
        return true;
    }

    private bool CheckSingleClick()
    {
        if(upTime - lastUpTime < singleClickIntervalTime) return false;
        onClick= OnSingleClick;
        return true;
    }

    private int _clickCount;
    private bool CheckDoubleClick()
    {
        if (upTime - lastUpTime > doubleClickIntervalTime)
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