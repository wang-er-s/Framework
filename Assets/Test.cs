using System;
using SF;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SF.UI.Core;
using SF.UI.Example;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Test : MonoBehaviour
{
    public SetupView view;

    private void Start()
    {
        view.ViewModel.Name.Value = "哈哈哈";
        view.ViewModel.Visible.Value = false;
    }

}


