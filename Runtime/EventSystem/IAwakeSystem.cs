namespace Framework
{
    public interface IAwakeSystem : ISystemType
    {
        void Awake();
    }

    public interface IAwakeSystem<A> : ISystemType
    {
        void Awake(A a);
    }

    public interface IAwakeSystem<A, B> : ISystemType
    {
        void Awake(A a, B b);
    }

    public interface IAwakeSystem<A, B, C> : ISystemType
    {
        void Awake(A a, B b, C c);
    }

    public interface IAwakeSystem<A, B, C, D> : ISystemType
    {
        void Awake(A a, B b, C c, D d);
    }
}