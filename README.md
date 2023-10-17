项目里使用了Odin插件，如有需要自行购买

## 特别鸣谢

感谢JetBrains公司提供的使用许可证！

<p><a href="https://www.jetbrains.com/?from=Framework ">
<img src="https://images.gitee.com/uploads/images/2020/0722/084147_cc1c0a4a_2253805.png" alt="JetBrains的Logo" width="20%" height="20%"></a></p>

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
