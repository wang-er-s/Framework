namespace Framework
{
    /// <summary>
    /// entity的引用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct EntityRef<T> where T : Entity
    {
        private T entity;
        private long entityId;

        public EntityRef(T entity) : this()
        {
            this.entity = entity;
            this.entityId = this.entity.Id;
        }

        public void SetEntity(T entity)
        {
            this.entity = entity;
            this.entityId = this.entity.Id;
        }

        public T Entity => entity;

        public bool IsDisposed
        {
            get
            {
                if (this.entity == null) return true;
                return entity.Id != entityId;
            }
        }

        public static implicit operator T(EntityRef<T> entityRef)
        {
            if (!entityRef.IsDisposed)
            {
                return entityRef.Entity;
            }

            return null;
        }

        public static implicit operator EntityRef<T>(T entity)
        {
            return new EntityRef<T>(entity);
        }
    }
}