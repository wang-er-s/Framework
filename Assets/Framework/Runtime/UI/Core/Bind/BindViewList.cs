using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.UI.Wrap;
using Framework.UI.Wrap.Base;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindViewList<TVm> : BaseBind where TVm : ViewModel
    {
        private Transform _content;
        private readonly List<View> _views;
        private readonly ObservableList<TVm> _list;
        private List<ViewWrapper> _wrappers;

        public BindViewList(ObservableList<TVm> list, params View[] view)
        {
            _views = view.ToList();
            _content = _views[0].transform.parent;
            this._list = list;
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            int childCount = _content.childCount;
            for (var i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                if(i + 1 < childCount)
                {
                    var view = _content.GetChild(i + 1).GetComponent<View>();
                    view.SetVm(vm);
                }
                else
                {
                    _wrappers.ForEach((wrapper) =>
                        ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));
                }
            }
        }

        private void InitEvent()
        {
            _wrappers = new List<ViewWrapper>(_views.Count);
            for (var i = 0; i < _views.Count; i++)
            {
                var wrapper = new ViewWrapper(_views[i]);
                wrapper.SetTag(i);
                _list.AddListener(((IBindList<ViewModel>) wrapper).GetBindListFunc());
                _views[i].Hide();
                _wrappers.Add(wrapper);
            }
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

        public BindIpairsViewList(ObservableList<TVm> list, string itemName, Transform root)
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
                var view = child.GetComponent<View>();
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