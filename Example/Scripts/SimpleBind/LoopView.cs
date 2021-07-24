using System.Collections.Generic;
using Framework.UI;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.UI;

public class LoopView : View
{
    [TransformPath("")]
    private LoopScrollRect loopVerticalScrollRect;

    private LoopVM vm;

    protected override void OnVmChange()
    {
        vm = ViewModel as LoopVM;
        Binding.BindViewList<Item_TestVM,Item_Test>(vm.items, loopVerticalScrollRect);
    }
}

public class LoopVM : ViewModel
{
    public ObservableList<Item_TestVM> items;

    public LoopVM(int count)
    {
        items = new ObservableList<Item_TestVM>();
        for (int i = 0; i < count; i++)
        {
            items.Add(new Item_TestVM(i.ToString()));
        }
    }
}

public class Item_Test : View
{
    [TransformPath("Text")]
    private Text text;
    [TransformPath("")]
    private Button button;

    private Item_TestVM vm;
    protected override void OnVmChange()
    {
        vm = ViewModel as Item_TestVM;
        Binding.Bind(text, vm.content);
        Binding.BindCommand(button, () => Debug.Log(vm.content));
    }
}

public class Item_TestVM : ViewModel
{
    public ObservableProperty<string> content;

    public Item_TestVM(string name)
    {
        content = new ObservableProperty<string>(name);
    }
}