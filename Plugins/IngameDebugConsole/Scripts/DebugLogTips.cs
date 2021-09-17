using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IngameDebugConsole
{
    public class DebugLogTips : MonoBehaviour
    {
        private static DebugLogTips ins;
        private CanvasGroup canvasGroup;
        [SerializeField]
        private Text text;
        private void Start()
        {
            ins = this;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public static void ShowTips(string text)
        {
            ins.canvasGroup.alpha = 1;
            ins.gameObject.SetActive(true);
            ins.text.text = text;
            ins.StartCoroutine(ins.Delay(2, () => ins.canvasGroup.alpha = 0));
        }
        
        IEnumerator Delay(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}