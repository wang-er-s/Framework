namespace Framework.Manager
{
    public interface IManager
    {
        void Init(params object[] para);
        void Update(float deltaTime);
    }
}