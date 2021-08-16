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

        public void Init(ConsoleMethodInfo consoleMethodInfo)
        {
            Input.placeholder.GetComponent<Text>().text = "请输入参数，类型在上面，每个参数用空格分隔";
            Input.gameObject.SetActive(consoleMethodInfo.parameterTypes.Length > 0);
            NameTxt.text = $">{consoleMethodInfo.command}({string.Join("",consoleMethodInfo.parameters)})";
            DescTxt.text = consoleMethodInfo.signature;
            Btn.onClick.AddListener(() =>
            {
                DebugLogConsole.ExecuteCommand($"{consoleMethodInfo.command} {Input.text}");
            });
        }
    }
}