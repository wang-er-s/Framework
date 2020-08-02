using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.UI.Wrap;
using Framework.UI.Wrap.Base;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindViewList<TVm> where TVm : ViewModel
    {
        private Transform content;
        private List<View> views;
        private BindableList<TVm> list;
        private List<ViewWrapper> wrappers;

        public BindViewList(BindableList<TVm> list, params View[] view)
        {
            views = view.ToList();
            content = views[0].transform.parent;
            this.list = list;
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            for (var i = 0; i < list.Count; i++)
            {
                var vm = list[i];
                wrappers.ForEach((wrapper) =>
                    ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));
            }
        }

        private void InitEvent()
        {
            wrappers = new List<ViewWrapper>(views.Count);
            for (var i = 0; i < views.Count; i++)
            {
                var wrapper = new ViewWrapper(views[i]);
                wrapper.SetTag(i);
                list.AddListener(((IBindList<ViewModel>) wrapper).GetBindListFunc());
                views[i].Hide();
                wrappers.Add(wrapper);
            }
        }
    }

    public class BindIpairsView<TVm> where TVm : ViewModel
    {
        private BindableList<TVm> list;
        private List<View> views;

        public BindIpairsView(BindableList<TVm> list, string itemName, Transform root)
        {
            SetValue(list, itemName, root);
        }

        private void SetValue(BindableList<TVm> list, string itemName, Transform root)
        {
            this.list = list;
            ParseItems(itemName, root);
            InitEvent();
        }
        
        private void ParseItems(string itemName, Transform root)
        {
            views = new List<View>();
            var regex = new Regex(@"[/w ]*?(?<=\[)[?](?=\])");
            if (!regex.IsMatch(itemName))
            {
                Debug.LogError($"{itemName} not match (skill[?]) pattern.");
                return;
            }
            int childCount = root.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var item = regex.Replace(itemName, i.ToString());
                View view = root.FindInAllChild(item)?.GetComponent<View>();
                if (view == null) break;
                views.Add(view);
            }
        }

        private void InitEvent()
        {
            for (var i = 0; i < views.Count; i++) views[i].SetVM(list[i]);
        }
    }
}