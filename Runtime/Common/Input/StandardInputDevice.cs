namespace Framework
{
    public class StandardInputDevice : InputDeviceBase
    {
        public override bool CanUse()
        {
            return FApplication.IsEditor || FApplication.IsPC;
        }
    }
}