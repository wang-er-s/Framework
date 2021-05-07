using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Framework.UI.Wrap;
using Framework.UI.Wrap.Base;
using Tool;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View
    {
        private Transform _content;
        private List<View> _views;
        private readonly ObservableList<TVm> _list;
        private List<ViewWrapper> _wrappers;

        public BindViewList(ObservableList<TVm> list, Transform root)  : base(null)
        {
            _views = new List<View>();
            _content = root;
            _list = list;
            InitEvent();
            InitCpntValue(); 
        }

        private void InitCpntValue()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                _wrappers.ForEach((wrapper) =>
                    ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));
            }
        }

        private void InitEvent()
        {
            int childCount = _content.childCount;
            _wrappers = new List<ViewWrapper>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                var view = ReflectionHelper.CreateInstance(typeof(TView)) as View;
                var wrapper = new ViewWrapper(view, _content,i);
                wrapper.SetTag(i);
                _list.AddListener(((IBindList<ViewModel>)wrapper).GetBindListFunc());
                _wrappers.Add(wrapper);
            }
        }

        public override void ClearBind()
        {
            _list.ClearListener();
        }
    }
    
    public class BindViewList<TVm> : BaseBind where TVm : ViewModel
    {
        private Transform _content;
        private List<View> _views;
        private readonly ObservableList<TVm> _list;
        private List<ViewWrapper> _wrappers;
        private Type _viewType;

        public BindViewList(ObservableList<TVm> list, Transform root, Type view): base(null)
        {
            if (!view.IsSubclassOf(typeof(View)))
            {
                Log.Error(view,"no subclass of View");
                return;
            }
            _viewType = view;
            _views = new List<View>();
            _content = root;
            _list = list;
            InitEvent();
            InitCpntValue(); 
        }

        private void InitCpntValue()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                _wrappers.ForEach((wrapper) =>
                    ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));
            }
        }

        private void InitEvent()
        {
            int childCount = _content.childCount;
            _wrappers = new List<ViewWrapper>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                var view = ReflectionHelper.CreateInstance(_viewType) as View;
                var wrapper = new ViewWrapper(view, _content,i);
                wrapper.SetTag(i);
                _list.AddListener(((IBindList<ViewModel>)wrapper).GetBindListFunc());
                _wrappers.Add(wrapper);
            }
        }

        public override void ClearBind()
        {
            _list.ClearListener();
        }
    }

    public class BindIpairsViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View
    {
        private ObservableList<TVm> _list;
        private List<View> _views;

        public BindIpairsViewList(ObservableList<TVm> list, string itemName, Transform root) : base(null)
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
            _views = new List<View>();
            var regex = new Regex(@"[/w ]*?(?<=\[)[?](?=\])");
            Log.Assert(regex.IsMatch(itemName), $"{itemName} not match (skill[?]) pattern.");
            foreach (Transform child in root)
            {

                var view = ReflectionHelper.CreateInstance(typeof(TView)) as View;
                view.SetGameObject (child.gameObject);
                Log.Assert(view != null, $"{child.name} must have view component", child);
                _views.Add(view);
            }
        }

        private void InitEvent()
        {
            for (var i = 0; i < _views.Count; i++) _views[i].SetVm(_list[i]);
        }

        public override void ClearBind()
        {
            _list.ClearListener();
        }
    }
    
    public class BindIpairsViewList<TVm> : BaseBind where TVm : ViewModel
    {
        private ObservableList<TVm> _list;
        private List<View> _views;
        private Type _viewType;

        public BindIpairsViewList(ObservableList<TVm> list, string itemName, Transform root, Type view) : base(null)
        {
            _viewType = view;
            this._list = list;
            ParseItems(itemName, root);
            InitEvent();
        }
        

        private void ParseItems(string itemName, Transform root)
        {
            _views = new List<View>();
            var regex = new Regex(@"[/w ]*?(?<=\[)[?](?=\])");
            Log.Assert(regex.IsMatch(itemName), $"{itemName} not match (skill[?]) pattern.");
            foreach (Transform child in root)
            {

                var view = ReflectionHelper.CreateInstance(_viewType) as View;
                view.SetGameObject (child.gameObject);
                Log.Assert(view != null, $"{child.name} must have view component", child);
                _views.Add(view);
            }
        }

        private void InitEvent()
        {
            for (var i = 0; i < _views.Count; i++) _views[i].SetVm(_list[i]);
        }

        public override void ClearBind()
        {
            _list.ClearListener();
        }
    }
}