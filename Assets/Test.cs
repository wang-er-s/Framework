using System;
using Nine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Nine.UI.Core;
using Nine.UI.Example;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Component = UnityEngine.Component;

public class Test : MonoBehaviour
{
    public SetupView view;
    public InputField inputField;

    private void Start()
    {
        view.ViewModel.Path = "img";
    }

}

class AA
{
    public string Name { get; set; }
}



