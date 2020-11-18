a unity mvvm framework, building...
项目里使用了Odin插件，如有需要自行购买
```c#
vm = ViewModel as SetupViewModel;
if(binding == null)
    binding = new UIBindFactory<SetupView, SetupViewModel>(this, vm);
binding.UpdateVm();
binding.Bind(nameMessageText, vm.Visible);
binding.Bind(nameMessageText, vm.Process, process => $"进度为:{process}");
binding.Bind (mulBindText, vm.Name, vm.ATK,
              (name, atk) => $"name = {name} atk = {atk.ToString ()}",(str)=>mulBindText.text = $"111{str}");
binding.Bind(joinInButton, vm.OnButtonClick, wrapFunc: click => () =>
{
    click();
    print("Wrap Button");
});
binding.Bind(joinInButton, () => vm.OnInputChanged("a"));
binding.RevertBind(slider, vm.Process);

binding.Bind (img, vm.Path);
binding.BindData(vm.Visible, vm.OnToggleChanged);
binding.RevertBind(joinToggle, vm.Visible);
binding.RevertBind(atkInputField, vm.ATK, (string str) => int.Parse(str));
```
