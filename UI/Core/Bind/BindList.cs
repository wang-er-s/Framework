using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Framework.UI.Wrap;
using Framework.UI.Core;
using UnityEngine;

namespace Framework.UI.Core
{
    public class BindList<TVm> where TVm : ViewModel
    {
        private Transform _content;
        private List<View> _views;
        private BindableList<TVm> _list;
        private List<ViewWrapper> _wrappers;

        public BindList (BindableList<TVm> list, params View[] view)
        {
            SetValue(list, view);
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                var vm = _list[i];
                _wrappers.ForEach((wrapper) =>
                    ((IBindList<ViewModel>) wrapper).GetBindListFunc()(NotifyCollectionChangedAction.Add, vm, i));

            }
        }

        private void SetValue(BindableList<TVm> list, params View[] view)
        {
            _views = view.ToList ();
            _content = _views[0].transform.parent;
            _list = list;
        }

        private void InitEvent()
        {
            _wrappers = new List<ViewWrapper> (_views.Count);
            for ( int i = 0; i < _views.Count; i++ )
            {
                var wrapper = new ViewWrapper(_views[i]);
                wrapper.SetTag(i);
                _list.AddListener(((IBindList<ViewModel>) wrapper).GetBindListFunc());
                _views[i].Hide();
                _wrappers.Add(wrapper);
            }
        }
    }

    public class BindIpairsView<TVm> where TVm : ViewModel
    {
        private BindableList<TVm> list;
        private List<View> views;

        public BindIpairsView (ref BindableList<TVm> _list, string itemName, Transform root)
        {
            SetValue(ref _list, itemName, root);
        }

        private void SetValue(ref BindableList<TVm> _list, string itemName, Transform root)
        {
            list = _list;
            ParseItems (itemName, root);
            InitEvent();
        }

        private void ParseItems (string itemName, Transform root)
        {
            views = new List<View> ();
            Regex regex = new Regex (@"[/w ]*?(?<=\[)[?](?=\])");
            if ( !regex.IsMatch (itemName) )
            {
                Debug.LogError ($"{itemName} not match (skill[?]) pattern.");
                return;
            }
            Transform upTransform = null;
            for ( int i = 0; i < Int32.MaxValue; i++ )
            {
                string item = regex.Replace (itemName, i.ToString ());
                View view;
                if ( upTransform == null )
                {
                    
                    view = root.FindInAllChild (item)?.GetComponent<View> ();
                    upTransform = view.transform.parent;
                }
                else
                {
                    view = upTransform.Find (item)?.GetComponent<View> ();
                    if ( view == null )
                    {
                        view = root.FindInAllChild (item)?.GetComponent<View> ();
                    }
                }
                if(view == null) break;
                views.Add(view);
            }
        }

        private void InitEvent()
        {
            for (int i = 0; i < views.Count; i++)
            {
                views[i].SetVM(list[i]);
            }
        }
    }
}
