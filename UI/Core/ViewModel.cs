using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.UI.Core
{
    public abstract class ViewModel
    {
        private const string MAIN_SCENE = "main_scene";
        private static Dictionary<string, UIManager> _uiManagers = new Dictionary<string, UIManager>();
        protected readonly IBindableProperty<bool> IsShow = new BindableProperty<bool>(false);
        public abstract string ViewPath { get;}

        public virtual void ShowView(string uiMgrTag = MAIN_SCENE)
        {
            IsShow.Value = true;
            if (_uiManagers.TryGetValue(uiMgrTag, out var uiManager))
                uiManager.Load(ViewPath, this);
        }
        
        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnDestroy()
        {
            
        }

        /// <summary>
        /// 当不同的view换绑vm的时候的reset方法
        /// </summary>
        public virtual void Reset()
        {
            
        }

        public void RegisterUIManager(UIManager uiManager, string tag = MAIN_SCENE)
        {
            if (_uiManagers.ContainsKey(tag))
            {
                _uiManagers[tag] = uiManager;
                return;
            }
            _uiManagers.Add(tag, uiManager);
        }

    }
}