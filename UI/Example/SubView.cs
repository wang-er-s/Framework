/*using System.Collections;
using System.Collections.Generic;
using Framework.UI.Core;
using Framework.UI.Example;
using UnityEngine;

public class SubView : View
{

    public SubViewModel viewModel;
  
    protected override void OnVmChange ()
    {
        viewModel = (SubViewModel) VM;
        BindFactory<SubView, SubViewModel> binding =
            new BindFactory<SubView, SubViewModel> (this, viewModel);
        binding.BindIpairs(viewModel.Items, "Skill [?]");
    }
    
}

public class SubViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items;

    public SubViewModel()
    {
        Items = new BindableList<ItemViewModel>();
    }
    
}*/