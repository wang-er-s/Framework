using System.Collections.Generic;

namespace Framework
{
    public sealed class Scene : Entity
    {
        public SceneType SceneType { get; }

        public string Name { get; }

        private Dictionary<string, Context> contexts = new();

        public Context SceneContext { get; private set; }

        public Scene(long instanceId, SceneType sceneType, string name, Entity parent)
        {
            SceneContext = new Context();
            Id = instanceId;
            SceneType = sceneType;
            Name = name;
            IsCreated = true;
            IsNew = true;
            Parent = parent;
            Domain = this;
            IsRegister = true;
            Log.Msg($"scene create: {SceneType} {Name} {Id} ");
        }

        public override void Dispose()
        {
            base.Dispose();
            SceneContext.Dispose();
            SceneContext = null;
            Log.Msg($"scene dispose: {SceneType} {Name} {Id}");
        }

        public new Entity Domain
        {
            get => domain;
            private set => domain = value;
        }

        public new Entity Parent
        {
            get => parent;
            private set
            {
                if (value == null)
                {
                    //this.parent = this;
                    return;
                }

                parent = value;
                parent.Children.Add(Id, this);
            }
        }


        public T GetContext<T>(string key) where T : Context
        {
            if (contexts.TryGetValue(typeof(T).Name, out Context context))
            {
                return context as T;
            }

            return null;
        }

        public T GetContext<T>() where T : Context
        {
            return GetContext<T>(typeof(T).Name);
        }

        public void AddContext<T>(T context) where T : Context
        {
            AddContext(context.GetType().Name, context);
        }

        public void AddContext(string key, Context context)
        {
            contexts.Add(key, context);
        }

        public void RemoveContext<T>()
        {
            RemoveContext(typeof(T).Name);
        }

        public void RemoveContext(string key)
        {
            if (contexts.TryGetValue(key, out Context context))
            {
                context.Dispose();
                contexts.Remove(key);
            }
        }

        protected override string ViewName => $"{GetType().Name} ({SceneType})";
    }
}