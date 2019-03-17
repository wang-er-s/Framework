using UnityEngine;

namespace ZFramework
{
    public interface IPanel
    {
        UIMoveType MoveType { get; }
        
        Transform Transform { get; }
		
        UIPanelInfo PanelInfo { get; set; }

        void Init ( IUIData uiData = null );
        
        void Show ();

        void Hide ();
        
        void Close (bool destroy = true);
    }
}