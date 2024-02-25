namespace Framework
{
    public class AutoUnRegisterComponent : Entity, IDestroySystem, IAwakeSystem
    {

        private RecyclableList<UnRegister> unRegisters;

        public void Awake()
        {
            unRegisters = RecyclableList<UnRegister>.Create();
        }

        public void AddUnRegister(UnRegister unRegister)
        {
            unRegisters.Add(unRegister);
        }

        public void OnDestroy()
        {
            foreach (UnRegister un in unRegisters)
            {
                un.Invoke();
            }

            unRegisters.Dispose();
        }
    }

    public static class AutoUnRegisterExtension
    {
        public static void AddUnRegister(this Entity entity, UnRegister unRegister)
        {
            var auto = entity.GetComponent<AutoUnRegisterComponent>();
            if (auto == null)
                auto = entity.AddComponent<AutoUnRegisterComponent>();
            auto.AddUnRegister(unRegister);
        }
    }
}