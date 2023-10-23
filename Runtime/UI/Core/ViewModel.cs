using System;

namespace Framework
{
    public abstract class ViewModel : Entity
    {
        private RecyclableList<IReference> references;
        
        public virtual void OnViewHide()
        {
        }
        
        public virtual void OnViewDestroy()
        {
            if(references == null) return;
            foreach (var reference in references)
            {
                ReferencePool.Free(reference);
            }
            references.Dispose();
        }

        protected T AllocateObservable<T>() where T : class, IObservable, IReference
        {
            if(references == null)
                references = RecyclableList<IReference>.Create();
            var result = ReferencePool.Allocate<T>();
            references.Add(result);
            return result;
        }
    }
}