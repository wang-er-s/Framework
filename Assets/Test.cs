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
    public Text text;
    private string outMsg;

    private void Start()
    {
         GetName((aa) => aa.Name);
         text.text = outMsg;
    }

    void GetName(Expression<Func<AA, string>> expression)
    {
        try
        {
            var body = expression.Body as MemberExpression;
            outMsg = body.Member.Name;
        }
        catch (Exception e)
        {
            outMsg = e.Message;
        }
    }

    void GetName2(LambdaExpression expression)
    {
        try
        {
            var body = expression.Body as MemberExpression;
            outMsg = body.Member.Name;
        }
        catch (Exception e)
        {
            outMsg = e.Message;
        }
    }

}

public class AA
{
    public string Name { get; set; }
}




