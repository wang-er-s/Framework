using UnityEngine;

namespace Framework.CommonHelper
{
    public static class PlatformHelper
    {
        private static bool? isIpad;

        public static bool IsPad
        {
            get
            {
                if (isIpad != null) return isIpad.Value;
                string type = SystemInfo.deviceModel.ToLower().Trim();
                switch (type.Substring(0, 3))
                {
                    case "iph":
                        //iPhone机型
                        isIpad = false;
                        break;
                    case "ipa":
                        //iPad机型
                        isIpad = true;
                        break;
                    default:
                        //其他
                        isIpad = false;
                        break;
                }

                return isIpad.Value;
            }
        }
    }
}
