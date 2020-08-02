using System;
using System.Collections.Generic;

namespace Framework.UI.Core
{
    public class VMCreator
    {
        private IUIViewLocator viewLocator;
        private Dictionary<ViewModel, View> vm2View = new Dictionary<ViewModel, View>();

        public VMCreator(IUIViewLocator viewLocator)
        {
            SetViewLocator(viewLocator);
        }

        public void SetViewLocator(IUIViewLocator viewLocator)
        {
            this.viewLocator = viewLocator;
        }

        public void BindView(ViewModel vm)
        {
            vm.IsShow.AddListener(show =>
            {
                switch (show)
                {
                    case ViewState.Show:
                        ShowView(vm);
                        break;
                    case ViewState.Hide:
                        HideView(vm);
                        break;
                    case ViewState.Destroy:
                        DestroyView(vm);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(show), show, null);
                }
            });
        }

        public void ChangeVm(View newView, ViewModel viewModel)
        {
            if (vm2View.ContainsKey(viewModel))
            {
                vm2View[viewModel] = newView;
                return;
            }
            vm2View.Add(viewModel, newView);
            newView.SetVM(viewModel);
        }

        private View ShowView(ViewModel viewModel)
        {
            if (!vm2View.TryGetValue(viewModel, out var view))
            {
                view = viewLocator.Load(viewModel.ViewPath, viewModel);
                vm2View.Add(viewModel, view);
            }
            view.Show();
            return view;
        }

        private void HideView(ViewModel viewModel)
        {
            if (vm2View.TryGetValue(viewModel, out var view)) view.Hide();
            Log.Warning($"{viewModel.ViewPath} window not show!");
        }

        private void DestroyView(ViewModel viewModel)
        {
            if (vm2View.TryGetValue(viewModel, out var view)) UnityEngine.Object.Destroy(view.gameObject);
            Log.Warning($"{viewModel.ViewPath} window not show!");
        }

        public View GetView(ViewModel viewModel)
        {
            return vm2View.TryGetValue(viewModel, out var view) ? view : null;
        }

    }
}