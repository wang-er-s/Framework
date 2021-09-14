using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IngameDebugConsole
{
    public class CommandButton : MonoBehaviour
    {
        [SerializeField]
        private Button Btn;

        [SerializeField]
        private InputField Input;

        [SerializeField]
        private Text NameTxt;

        [SerializeField] 
        private Text DescTxt;

        private Image btnImg;
        private string oldName;

        private void Start()
        {
            btnImg = Btn.GetComponent<Image>();
        }

        public void Init(ConsoleMethodInfo consoleMethodInfo)
        {
            Input.placeholder.GetComponent<Text>().text = "请输入参数，类型在上面，每个参数用空格分隔";
            Input.gameObject.SetActive(consoleMethodInfo.parameterTypes.Length > 0);
            oldName = $">{consoleMethodInfo.command}({string.Join("",consoleMethodInfo.parameters)})";
            NameTxt.text = oldName;
            DescTxt.text = consoleMethodInfo.description;
            Btn.onClick.AddListener(() =>
            {
                DebugLogConsole.ExecuteCommand($"{consoleMethodInfo.command} {Input.text}");
                btnImg.color = Color.green;
                NameTxt.text = "执行成功！！";
                StartCoroutine(Delay(1, () =>
                {
                    btnImg.color = Color.white;
                    NameTxt.text = oldName;
                }));
            });
        }

        IEnumerator Delay(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}