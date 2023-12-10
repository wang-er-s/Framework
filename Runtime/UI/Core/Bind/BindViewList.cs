using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Framework
{
    public class BindViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View
    {
        private Transform _content;
        private List<View> _views;
        private ObservableList<TVm> _list;
        private Type viewType;
        private Window parentView;

        public void Reset(ObservableList<TVm> list,Window parent, Transform root)
        {
            _views = new List<View>();
            parentView = parent;
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
            // 占位，防止一帧多次创建
            var emptyGo = parentView.Domain.GetComponent<PrefabPool>().AllocateSync(String.Empty);
            emptyGo.transform.SetParent(_content);
            emptyGo.transform.SetSiblingIndex(index + 1);
            parentView.AddSubView<TView>(vm).Callbackable().OnCallback(r =>
            {
                var view = r.Result;
                var go = r.Result.GameObject;
                go.transform.SetParent(_content);
                go.transform.SetSiblingIndex(index + 1);
                parentView.Domain.GetComponent<PrefabPool>().Free(emptyGo);
                go.ActiveShow();
                view.SetGameObject(go);
                view.SetVm(vm);
                view.Visibility = true;
                _views.Insert(index, view);
            });
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

    public class BindIpairsViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View
    {
        private ObservableList<TVm> _list;
        private List<View> _views;
        private Type viewType;
        private Window parentWindow;

        public void SetViewType(Type type,Window parent)
        {
            viewType = type;
            parentWindow = parent;
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

        private async void ParseItems(string itemName, Transform root)
        {
            _views = new List<View>();
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
                    var view = await parentWindow.Domain.GetComponent<UIComponent>().CreateSubViewAsync<TView>(null);
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