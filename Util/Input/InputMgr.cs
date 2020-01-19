using System.Collections.Generic;
using Framework;
using UnityEngine;

public class InputMgr : MonoBehaviour
{
    private static InputMgr _instance;
    private Stack<ISetInput> _inputStack;
    private ISetInput _curSetInput;
    private InputData _inputData;

    private void Awake()
    {
        _instance = this;
        _inputStack = new Stack<ISetInput>();
        _inputData = new InputData();
    }

    void Update()
    {
        if(_curSetInput == null) return;
        _inputData.Reset();
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Input.GetMouseButton(0))
        {
            float xDrag = Input.GetAxis("Mouse X");
            float yDrag = Input.GetAxis("Mouse Y");
            _inputData.Drag = new Vector2(xDrag, yDrag);
        }
        _inputData.Horizontal_Vertical = new Vector2(horizontal, vertical);
        _curSetInput.SetInput(_inputData);
    }

    public static void Init(ISetInput setInput)
    {
        if (_instance == null)
            _instance = MonoSingletonCreator.CreateMonoSingleton<InputMgr>();
        SetInput(setInput);
    }

    public static void SetInput(ISetInput setInput)
    {
        _instance._inputStack.Push(setInput);
        _instance._curSetInput = setInput;
    }

    public static void RemoveInput()
    {
        if (_instance._inputStack.Count <= 0)
        {
            _instance.LogWarning("Input stack is null!");
            return;
        }
        _instance._inputStack.Pop();
        _instance._curSetInput = _instance._inputStack.Peek();
    }
    
}

public class InputData
{
    public Vector2 Horizontal_Vertical;
    public Vector2 Drag;

    public void Reset()
    {
        Horizontal_Vertical = Vector2.zero;
        Drag = Vector2.zero;
    }
}

public interface ISetInput
{
    void SetInput(InputData inputData);
}
