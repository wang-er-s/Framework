/*
using System.Collections;
using System.Collections.Generic;
using AD.UI.Core;
using AD.UI.Example;
using UnityEngine;

public class ListPairsBindView : View
{
    private ListPairsBindViewModel viewModel;
    protected override void OnVmChange()
    {
        viewModel = VM as ListPairsBindViewModel;
        BindFactory<ListPairsBindView, ListPairsBindViewModel> binding =
            new BindFactory<ListPairsBindView, ListPairsBindViewModel>(this, viewModel);
        binding.BindIpairs(viewModel.Items,"item[?]").InitBind();
    }
}

public class ListPairsBindViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items { get; private set; }

    public ListPairsBindViewModel()
    {
        Items = new BindableList<ItemViewModel>()
        {
            new ItemViewModel(){Path = new BindableField<string>("回锅肉")},
            new ItemViewModel(){Path = new BindableField<string>("梅菜扣肉")},
            new ItemViewModel(){Path = new BindableField<string>("水煮鱼")},
            new ItemViewModel(){Path = new BindableField<string>("鱼香肉丝")},
        };
    }
}
*/
