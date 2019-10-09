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
    private event Action a1;

    private void Start()
    {
        AA a1 = new AA();
        DoUI(a1.Name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            view.viewModel.Items.Add(new ItemViewModel() { Path = "img" });
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            view.viewModel.Items.RemoveAt(0);
        }
    }

    protected void DoUI(object property)
    {
        Type type = property.GetType();
        Debug.Log("Type ToString: " + type.ToString() + " | Name: " + type.Name + " | Full Name: " + type.FullName);
    }

}

class AA : INotifyPropertyChanged
{
    public string Name { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}



