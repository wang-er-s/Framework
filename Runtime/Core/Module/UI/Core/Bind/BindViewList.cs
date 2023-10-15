using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Framework
{
    public class BindViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : Window
    {
        private Transform _content;
        private List<Window> _views;
        private ObservableList<TVm> _list;
        private Type viewType;

        private BindViewList()
        {
        }

        public void Reset(ObservableList<TVm> list, Transform root)
        {
            _views = new List<Window>();
            _content = root;
            _list = list;
            viewType = typeof(TView);
            InitEvent();
            InitCpntValue(); 
        }

        private void InitCpntValue()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                BindListFunc(NotifyCollectionChangedAction.Add, vm, i);
            }
        }

        private void InitEvent()
        {
            _list.AddListener(BindListFunc);
        }

        protected override void OnReset()
        {
            _list.RemoveListener(BindListFunc);
        }
        
        private void BindListFunc
            (NotifyCollectionChangedAction type, ViewModel newViewModel, int index)
        {
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItem(index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItem(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItem(index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear(index);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void AddItem(int index, ViewModel vm)
        {
            var view = UIComponent.Instance.AddChild(viewType) as Window;
            var go = UIComponent.Instance.CreateViewGameObject(viewType);
            go.transform.SetParent(_content);
            go.transform.SetSiblingIndex(index + 1);
            go.ActiveShow();
            view.SetGameObject(go);
            view.SetVm(vm);
            view.Show();
            _views.Insert(index, view);
        }

        private void RemoveItem(int index)
        {
            var view = _views[index];
            _views.RemoveAt(index);
            view.Dispose();
        }

        private void ReplaceItem(int index, ViewModel vm)
        {
            _views[index].SetVm(vm);
        }

        private void Clear(int itemCount)
        {
            while (itemCount > 0)
            {
                RemoveItem(--itemCount);
            }

            _list.RemoveListener(BindListFunc);
        }

        protected override void OnClear()
        {
            _content = default;
            _views = default;
            _list = default;
            viewType = default;
        }
    }

    public class BindIpairsViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : Window
    {
        private ObservableList<TVm> _list;
        private List<Window> _views;
        private Type viewType;

        private BindIpairsViewList()
        {
        }

        public void SetViewType(Type type)
        {
            viewType = type;
        }

        public void Reset(ObservableList<TVm> list, string itemName, Transform root)
        {
            SetValue(list, itemName, root);
        }

        private void SetValue(ObservableList<TVm> list, string itemName, Transform root)
        {
            this._list = list;
            ParseItems(itemName, root);
            InitEvent();
        }

        private void ParseItems(string itemName, Transform root)
        {
            _views = new List<Window>();
            var regex = new Regex(@"(\w+)?\[\?\]");
            var match = regex.Match(itemName);
            Log.Assert(match.Success, $"{itemName} not match (skill[?]) pattern.");
            itemName = match.Groups[1].Value;
            regex = new Regex(itemName + @"\[\d+\]");
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (regex.IsMatch(child.name))
                {
                    var view = UIComponent.Instance.AddChild(viewType) as Window;
                    view.SetGameObject(child.gameObject);
                    _views.Add(view);
                }
            }
        }

        private void InitEvent()
        {
            for (var i = 0; i < _views.Count; i++) _views[i].SetVm(_list[i]);
        }

        protected override void OnReset()
        {
            foreach (var view in _views)
            {
                view.Dispose();
            }
        }

        protected override void OnClear()
        {
            _list = default;
            _views = default;
            viewType = default;
        }
    }
}