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

        public T Entity
        {
            get
            {
                if (entity == null)
                {
                    return null;
                }
                
                if (entity.IsDisposed || entity.Id != entityId)
                {
                    entity = null;
                    return null;
                }

                return entity;
            }
        }

        public static implicit operator T(EntityRef<T> entityRef)
        {
            return entityRef.Entity;
        }

        public static implicit operator EntityRef<T>(T entity)
        {
            return new EntityRef<T>(entity);
        }
    }
}