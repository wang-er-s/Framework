using System;
using SF;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SF.UI.Core;
using SF.UI.Example;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public SetupView view;
    public InputField inputField;

    private void Start()
    {
        //view.ViewModel.Name.Value = "哈哈哈";
        //view.ViewModel.Visible.Value = false;
        //Exp((t)=>t.Function);
    }

    private void Function()
    {

    }

    private void Exp<T>(Expression<Func<Test,T>> expression)
    {

    }

}

class AA : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}



