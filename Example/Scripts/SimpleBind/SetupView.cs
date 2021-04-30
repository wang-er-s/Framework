using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Example
{
	public class SetupView : View
	{
		[TransformPath("name")] private Text nameMessageText;
		
		[TransformPath("mulBind")] private Text mulBindText;
		
		[TransformPath("InputField")] private InputField atkInputField;

		[TransformPath("Toggle")] private Toggle joinToggle;

		[TransformPath("Button")] private Button joinInButton;

		[TransformPath("Image")] private Image img;

		[TransformPath("Slider")] private Slider slider;

		private SetupViewModel vm;
		private View subView;

		protected override void OnVmChange()
		{
			vm = ViewModel as SetupViewModel;
			Binding.Bind(nameMessageText, vm.Visible);
			Binding.Bind(nameMessageText, vm.Process, process => $"进度为:{process}");
			Binding.Bind(mulBindText, vm.Name, vm.ATK,
				(name, atk) => $"name = {name} atk = {atk.ToString()}", (str) => mulBindText.text = $"111{str}");
			//如果绑定的是Action 则需要用闭包，下面修改OnClick方法才会实时应用上，因为Action传递是新建了一个Action并把方法传过去
			//可以把Action当成是值类型
			Binding.BindCommand(joinInButton, () => vm.OnClick());
			Binding.RevertBind(slider, vm.Process);
			Binding.Bind(img, vm.Path);
			Binding.BindData(vm.Visible, vm.OnToggleChanged);
			Binding.RevertBind(joinToggle, vm.Visible);
			Binding.RevertBind(atkInputField, vm.ATK, (string str) => int.Parse(str));
			vm.OnClick += () => Debug.Log(222);
			Debug.Log(vm.OnClick.GetHashCode());
		}

		public override UILevel UILevel { get; } = UILevel.Common;
	}
}