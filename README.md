项目里使用了Odin插件，如有需要自行购买
文档<https://www.yuque.com/books/share/2c4b21e9-f8e8-4841-92ff-e6fdb69b0e16?# 《框架使用指南》>
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
