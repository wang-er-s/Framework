using System.Collections.Generic;

namespace Framework.Editor.AssetsChecker
{
    public class BasicAssetCheckerCollection<T> : CheckerCollection where T : BasicAssetCheckerCollection<T>
    {
        public override string Name => "基本资源检查";
        protected List<IRule<T>> rules;
    }
}