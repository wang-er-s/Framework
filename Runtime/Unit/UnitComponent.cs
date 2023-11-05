using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public class UnitComponent : Entity, IAwakeSystem, IDestroySystem
    {
        private Dictionary<long, Unit> idUnits = new Dictionary<long, Unit>();
        public IReadOnlyDictionary<long, Unit> Units => idUnits;

        public void Awake()
        {
        }

        public void Add(Unit unit)
        {
            idUnits.Add(unit.Id, unit);
        }

        public Unit Get(long id)
        {
            Unit unit;
            idUnits.TryGetValue(id, out unit);
            return unit;
        }

        public void Remove(long id)
        {
            Unit unit;
            idUnits.TryGetValue(id, out unit);
            idUnits.Remove(id);
            unit?.Dispose();
        }

        public void RemoveAll()
        {
            foreach (var unit in idUnits)
            {
                unit.Value?.Dispose();
            }

            idUnits.Clear();
        }

        public void RemoveNoDispose(long id)
        {
            idUnits.Remove(id);
        }

        public void OnDestroy()
        {
        }
    }
}