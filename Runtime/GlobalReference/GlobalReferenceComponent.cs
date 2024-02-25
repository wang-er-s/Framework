using Framework;
using UnityEngine;

public class GlobalReferenceComponent : Entity , IAwakeSystem
{
    public static GlobalReferenceComponent Instance { get; private set; }

    public Transform UnitRoot{ get; private set; }
    public Canvas UICanvas{ get; private set; }
    public Transform BgUIRoot { get; private set; }
    public Transform CommonUIRoot { get; private set; }
    public Transform PopUIRoot { get; private set; }
    public Transform GuideUIRoot { get; private set; }
    public Transform FullScreenUIRoot { get; private set; }
    
    public void Awake()
    {
        Instance = this;
        UnitRoot = GameObject.Find("UnitRoot").transform;
        UICanvas = GameObject.Find("UIRoot").GetComponentInChildren<Canvas>();
        BgUIRoot = UICanvas.transform.Find("Bg");
        CommonUIRoot = UICanvas.transform.Find("Common");
        PopUIRoot = UICanvas.transform.Find("Pop");
        GuideUIRoot = UICanvas.transform.Find("Guide");
        FullScreenUIRoot = UICanvas.transform.Find("FullScreen");
    }
}
