namespace Framework
{
    public interface IDomain
    {
        int Name { get; }
        bool IsLoad { get;  }
        void BeginEnter(object data = null);
        void BeginExit();
    }
}