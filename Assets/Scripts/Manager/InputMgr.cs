using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZFramework;

public class InputMgr : MonoSingleton<InputMgr>
{

    public bool IsForward;
    public bool IsBack;
    public bool IsRight;
    public bool IsLeft;
    public bool IsJump;
    public bool IsCameraLeftRotate;
    public bool IsCameraRightRotate;
    public bool IsAttack;
    
    
    void Update()
    {
        switch (ConstData.InputType)
        {
            case InputType.Windows:
                WinInput();
                break;
            case InputType.Phone:
                PhoneInput();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void WinInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            IsForward = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            IsForward = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            IsBack = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            IsBack = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            IsLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            IsLeft = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            IsRight = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            IsRight = false;
        }
        IsJump = false || Input.GetKeyDown(KeyCode.K);
        IsAttack = false || Input.GetKeyDown(KeyCode.J);
    }

    void PhoneInput()
    {
        
    }
}
