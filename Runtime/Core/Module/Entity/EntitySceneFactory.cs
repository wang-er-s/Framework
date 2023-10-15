namespace Framework
{
    public static class EntitySceneFactory
    {
        public static Scene CreateScene(SceneType sceneType,Entity parent = null)
        {
            long instanceId = IdGenerator.Instance.GenerateInstanceId();
            Scene scene = new Scene(instanceId, sceneType, sceneType.ToString(), parent);
            if (sceneType != SceneType.Root)
            {
                scene.AddComponent<ResComponent>();
                scene.AddComponent<PrefabPool, ResComponent,string>(scene.GetComponent<ResComponent>(),$"{sceneType}_PrefabPool");
            }
            return scene;
        }
    }
}