namespace Framework
{
    public interface IDomain
    {
        int Name { get; }
        bool IsLoad { get;  }
        void BeginEnter();
        void BeginExit();
    }
}