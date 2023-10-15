namespace Framework
{
    public static class SceneHelper
    {
        public static Scene DomainScene(this Entity entity)
        {
            return (Scene)entity.Domain;
        }

        public static Scene RootScene(this Entity entity)
        {
            return Root.Instance.Scene;
        }
    }

    public struct SceneChangedData
    {
        public SceneType OldType;
        public SceneType NewType;

        public SceneChangedData(SceneType oldType, SceneType newType)
        {
            OldType = oldType;
            NewType = newType;
        }
    }
}