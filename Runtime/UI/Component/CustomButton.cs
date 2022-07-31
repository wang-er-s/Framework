using System;
using Framework.UI.Wrap.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UIComponent
{
    public class CustomButton : Button, IComponentEvent, IFieldChangeCb<bool>
    {
        [SerializeField] private float singleClickIntervalTime = 0.3f;
        [SerializeField] private float doubleClickIntervalTime = 0.3f;
        [SerializeField] private float longClickTime = 1;
        [SerializeField] private float longPressIntervalTime = 0.3f;
        private bool isPointDown = false;
        private float lastInvokeTime;

        private float lastUpTime;
        private float lastDownTime;
        private float downTime;
        private float upTime;

        private static Material grayMat;
        private Material selfMat;
        private CanvasRenderer canvasRenderer;
        private Text textComponent;
        private TextMeshProUGUI tmp;
        private string text;

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
                //缓存一下，避免awake没执行
                text = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            canvasRenderer = GetComponent<CanvasRenderer>();
            textComponent = GetComponentInChildren<Text>();
            tmp = GetComponentInChildren<TextMeshProUGUI>();
            if (!string.IsNullOrEmpty(text))
            {
                if (textComponent != null) textComponent.text = text;
                if (tmp != null) tmp.text = text;
            }

            selfMat = targetGraphic.material;
        }

        public ButtonClickedEvent OnSingleClick { get; } = new ButtonClickedEvent();
        public ButtonClickedEvent OnDoubleClick { get; } = new ButtonClickedEvent();
        public UnityEvent OnLongClick { get; } = new UnityEvent();

        public UnityEvent OnLongPress { get; } = new UnityEvent();
        public UnityEvent OnDown { get; } = new UnityEvent();
        public UnityEvent OnUp { get; } = new UnityEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!IsInteractable()) return;
            lastDownTime = downTime;
            downTime = Time.time;
            OnDown.Invoke();
            isPointDown = true;
            lastInvokeTime = downTime;
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            isPointDown = false;
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!IsInteractable()) return;
            var time = Time.time;
            lastUpTime = upTime;
            upTime = time;
            OnUp.Invoke();
            CheckDoubleClick();
            CheckLongClick();
            isPointDown = false;
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
            if (upTime - lastUpTime < singleClickIntervalTime) return false;
            OnSingleClick.Invoke();
            return true;
        }

        private int clickCount;

        private bool CheckDoubleClick()
        {

            if (upTime - lastUpTime > doubleClickIntervalTime)
            {
                clickCount = 1;
                return false;
            }

            clickCount++;
            if (clickCount >= 2)
            {
                OnDoubleClick.Invoke();
                clickCount = 0;
                return true;
            }

            return false;
        }
        
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            CheckSingleClick();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            
            if (!IsActive() || !IsInteractable())
                return;

            CheckSingleClick();
        }

        public UnityEvent GetComponentEvent()
        {
            return OnSingleClick;
        }

        public Action<bool> GetFieldChangeCb()
        {
            return value => gameObject.SetActive(value);
        }

        void Update()
        {
            if (!isPointDown) return;
            if (!(Time.time - downTime >= longClickTime)) return;
            if (Time.time - lastInvokeTime > longPressIntervalTime)
            {
                OnLongPress.Invoke();
                lastInvokeTime = Time.time;
            }
        }
    }
}