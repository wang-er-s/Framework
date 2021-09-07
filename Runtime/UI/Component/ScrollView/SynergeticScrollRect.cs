using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SynergeticScrollRect : ScrollRect
{
    private ScrollRect fatherScrollRect;

    public void SetFatherScrollRect(ScrollRect fatherScrollRect)
    {
        this.fatherScrollRect = fatherScrollRect;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (fatherScrollRect == null)
            return;
        fatherScrollRect.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (fatherScrollRect == null)
            return;
        fatherScrollRect.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (fatherScrollRect == null)
            return;
        fatherScrollRect.OnEndDrag(eventData);
    }
}