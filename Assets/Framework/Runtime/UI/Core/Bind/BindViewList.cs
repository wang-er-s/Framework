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
            for (var i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                _wrappers.ForEach((wrapper) =>
                    ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));
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

    public class BindIpairsView<TVm> : BaseBind where TVm : ViewModel
    {
        private ObservableList<TVm> _list;
        private List<View> _views;

        public BindIpairsView(ObservableList<TVm> list, string itemName, Transform root)
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
            if (!regex.IsMatch(itemName))
            {
                Debug.LogError($"{itemName} not match (skill[?]) pattern.");
                return;
            }

            int childCount = root.childCount;
            for (var i = 1; i <= childCount; i++)
            {
                var item = regex.Replace(itemName, i.ToString());
                View view = root.FindInAllChild(item)?.GetComponent<View>();
                if (view == null)
                {
                    Debug.LogError($"{item} do not have view component");
                    break;
                }
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