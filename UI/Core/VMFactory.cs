using System;
using System.Collections.Generic;

namespace Framework.UI.Core
{
    public class VMFactory
    {
        private UIManager _uiManager;
        private Dictionary<ViewModel, View> _vm2View = new Dictionary<ViewModel, View>();

        public VMFactory(UIManager uiManager)
        {    
            SetUIManager(uiManager);
        }
        
        public void SetUIManager(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public T Create<T>() where T : ViewModel,new()
        {
            T vm = new T();
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
            
            return vm;
        }

        private View ShowView(ViewModel viewModel)
        {
            if (!_vm2View.TryGetValue(viewModel, out var view))
            {
                view = _uiManager.Load(viewModel.ViewPath, viewModel);
                _vm2View.Add(viewModel, view);
            }
            view.Show();
            return view;
        }

        private void HideView(ViewModel viewModel)
        {
            if (_vm2View.TryGetValue(viewModel, out var view))
            {
                view.Hide();
            }
            Debugger.Warning($"{viewModel.ViewPath} window not show!");
        }

        private void DestroyView(ViewModel viewModel)
        {
            if (_vm2View.TryGetValue(viewModel, out var view))
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
            Debugger.Warning($"{viewModel.ViewPath} window not show!");
        }

        public View GetView(ViewModel viewModel)
        {
            return _vm2View.TryGetValue(viewModel, out var view) ? view : null;
        }
        
    }
}