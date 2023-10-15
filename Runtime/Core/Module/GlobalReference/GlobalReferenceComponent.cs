using Framework;
using UnityEngine;

public class GlobalReferenceComponent : Entity , IAwakeSystem
{
    public static GlobalReferenceComponent Instance { get; private set; }

    public Transform UnitRoot{ get; private set; }
    public Canvas UICanvas{ get; private set; }
    
    public void Awake()
    {
        Instance = this;
        UnitRoot = GameObject.Find("UnitRoot").transform;
        UICanvas = GameObject.Find("UIRoot").GetComponent<Canvas>();
    }
}
