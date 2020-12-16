### 什么是mvvm
- https://www.oschina.net/translate/wpf-mvvm-step-by-step-basics-to-advance-level?print
### 代码流程图
![avatar](http://assets.processon.com/chart_image/5f0ec5c07d9c081beab7bba2.png)

### 实例
```c#
//account是int类型，由于默认textWrapper实现了int的转换，所以可以直接使用，具体代码在TextWrapper中
Binding.Bind(_txt, _vm.Account);

//反向绑定，默认绑定是数据到组件，我们想要组件来控制数据就需要反向绑定
Binding.RevertBind(_toggle, _vm.IsToggle);

//双向绑定，inputField可以控制account，account改变同样会通知到input组件
Binding.TwoWayBind(_inputAccount, _vm.Account);

//绑定方法
Binding.BindCommand(_btnConfirm, _vm.Confirm);
```
- 上述各个方法都有很多缺省的参数：
- propChangeCb：字段改变的回调，如果不传会去找对应组件的wrapper来找到合适的回调，如Text.text
- componentEvent：反向绑定或者绑定方法时需要提供的组件Event，不传会找对应wrapper，如button.onClick，toggle.onValueChanged
- prop2CpntWrap：有时候vm那边提供的类型与view需要的类型不符，如想把年龄显示到ui上，则需要转换成string，这时候就需要这个参数。还有一种情况是想把view那边提供的参数加上一些修饰，如那边提供的年龄 "1"，而我们想显示的是 ”年龄=1“，则需要这个参数
- cpnt2FieldWrap：如上，view提供的值与vm的预期不符时

### 特殊使用
- BindViewList、BindIpairs和BindDropDown 都在特殊情况下使用，具体用法可以查看案例。热更项目里提供的现在版本的使用，需要把案例改成现有格式才能正常运行

### UIManager
```c#
// 主项目内主要使用的打开界面接口，根据View.Path来加载界面，vm是外部传入，还是界面打开时start自动创建看具体需求
internal IProgressResult<float, T> OpenAsync<T>(ViewModel viewModel = null)

// 热更项目主要使用的打开界面接口，因热更技术原因，需要传入type才能创建对应的类而不能使用泛型
public IProgressResult<float, View> OpenAsync(Type type, ViewModel viewModel = null)

// 同步加载接口，不建议使用，目前项目全部使用异步加载
public View Open(string path, ViewModel viewModel = null)

// GetView
internal T Get<T>() where T : View
public View Get(Type type)
```

### 自动生成代码（暂时不能使用）
- 在需要使用的组件挂上UIMark
- 然后拖成prefab，右键点击prefab->@UI Kit - Create UI Code 自动生成代码模板
