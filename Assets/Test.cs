using System;
using AD;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AD.UI.Core;
using JetBrains.Annotations;
using AD.UI.Core;
using AD.UI.Example;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;
[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public SubView view;

    private void Start ()
    {
        AA<Component> (null);
    }

    private void Update ()
    {
        if ( Input.GetKeyDown (KeyCode.S) )
        {
            view.viewModel.Items[0].Path = "img";
        }
    }

    private void AA<T> (T aa) where T : Component
    {
        BindFunc<T> Empty = new BindFunc<T> (aa, null);
    }
}






