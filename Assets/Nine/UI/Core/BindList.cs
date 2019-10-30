using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nine.UI.Wrap;
using Nine;
using Nine.UI.Core;
using UnityEngine;

namespace Assets.Nine.UI.Core
{
    public class BindList<TVm> : IBindSet where TVm : ViewModel
    {
        private Transform content;
        private List<View> views;
        private List<int> tags;
        private BindableList<TVm> list;
        private List<ViewWrapper> wrappers;

        public BindList (BindableList<TVm> _list, params View[] _view)
        {
            views = _view.ToList ();
            content = views[0].transform.parent;
            list = _list;
        }

        public BindList<TVm> SetTag (params int[] _tags)
        {
            tags = _tags.ToList ();
            if(tags.Count != views.Count)
                Log.Error("Tag must have the same length as view");
            return this;
        }
        
        public void Init()
        {
            wrappers = new List<ViewWrapper> (views.Count);
            for ( int i = 0; i < views.Count; i++ )
            {
                var wrapper = new ViewWrapper(views[i]);
                wrapper.SetTag (tags?[i] ?? i);
                IBindList<ViewModel> bindList = wrapper;
                list.AddListener(bindList.GetBindListFunc());
                wrappers.Add(wrapper);
            }
        }
    }
}
