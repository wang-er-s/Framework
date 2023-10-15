using Sirenix.OdinInspector;
using UnityEngine;

public class GoConnectedUnitId : MonoBehaviour
{
    [ShowInInspector]
    public long UnitId { get; private set; }

    public void SetUnitId(long unitId)
    {
        UnitId = unitId;
    }
}