namespace Framework
{
    public interface ILateUpdateSystem : ISystemType
    {
        void LateUpdate(float deltaTime);
    }
}