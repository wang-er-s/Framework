namespace Framework
{
    public interface IFixedUpdateSystem : ISystemType
    {
        void FixedUpdate(float deltaTime);
    }
}