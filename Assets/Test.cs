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
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;

public class Test : MonoBehaviour
{
    public SetupView view;

    private void Start ()
    {
    }

    private void Update ()
    {
        if ( Input.GetKeyDown ( KeyCode.Space ) )
        {
            view.viewModel.Process = 0.4f;
        }
    }
}






