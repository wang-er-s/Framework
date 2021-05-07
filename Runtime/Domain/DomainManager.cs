using System;
using System.Collections.Generic;

namespace Framework
{
    public class DomainManager : ManagerBase<DomainManager, DomainAttribute, int>
    {
        Dictionary<int, IDomain> _allDomains = new Dictionary<int, IDomain>();
        private int _defaultScreenTag = 0;
        public IDomain CurrentDomain { get; private set; }

        public override void Init()
        {
            foreach (var classData in this.GetAllClassDatas())
            {
                var attr = classData.Attribute as DomainAttribute;

                var sv = CreateInstance<IDomain>(attr.IntTag);
                //设置name属性
                var t = sv.GetType();
                t.GetProperty(nameof(IDomain.Name))?.SetValue(sv, attr.IntTag, null);
                RegisterScreen(sv);
            }
        }

        public override void Start()
        {
            BeginNavTo(_defaultScreenTag);
        }
        
        private void RegisterScreen(IDomain domain)
        {
            _allDomains.Add(domain.Name, domain);
        }

        public IDomain GetDomain(int svName)
        {
            IDomain sv = null;
            foreach (var _sv in _allDomains.Values)
            {
                if (_sv.Name == svName)
                {
                    sv = _sv;
                    break;
                }
            }
            return sv;
        }

        public IDomain GetDomain(Enum name)
        {
            return GetDomain(name.GetHashCode());
        }

        public void BeginNavTo(Enum name)
        {
            BeginNavTo(name.GetHashCode());
        }

        public void BeginNavTo(int name)
        {
            if (CurrentDomain != null && CurrentDomain.Name == name)
            {
                Log.Error("别闹，当前就是" + CurrentDomain.GetType().Name);
                return;
            }
            
            if (_allDomains.TryGetValue(name, out var domain))
            {
                domain.BeginEnter();
                CurrentDomain?.BeginExit();
                CurrentDomain = domain;
            }
        }
    }
}