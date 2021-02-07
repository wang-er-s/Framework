using System;

namespace Framework
{
    public interface IManager
    {
        void Init();
        void Start();
        void CheckType(Type type);
    }
    
    public class ClassData
    {
        public ManagerAttribute Attribute;
        public Type Type;
    }
}