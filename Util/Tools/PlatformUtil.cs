using UnityEngine;

namespace Framework
{
	public class PlatformUtil  
	{

        public static bool IsIpad()
        {
            bool isLandscape = Screen.width > Screen.height;
            float aspect;
            if (isLandscape)
                aspect = (float)Screen.width / Screen.height;
            else
                aspect = (float)Screen.height / Screen.width;
            if (aspect > (4.0f / 3 - 0.05) && aspect < (4.0f))
                return true;
            return false;
        }

	}
}
