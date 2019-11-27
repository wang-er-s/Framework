using System.Collections;
using System.Collections.Generic;
using AD.UI.Core;
using AD.UI.Core;
using AD.UI.Example;
using UnityEngine;

public class SubView : View
{

    public SubViewModel viewModel;
    
    protected override void OnCreate ()
    {
        viewModel = (SubViewModel) data;
        BindFactory<SubView, SubViewModel> binding =
            new BindFactory<SubView, SubViewModel> (this, viewModel);
        binding.BindIpairs(viewModel.Items, "Skill [?]").Init();
    }

    protected override ViewModel CreateVM ()
    {
        return new SubViewModel();
    }
    
}

public class SubViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items;

    public override void OnCreate ()
    {
        Items = new BindableList<ItemViewModel> ();
    }
}