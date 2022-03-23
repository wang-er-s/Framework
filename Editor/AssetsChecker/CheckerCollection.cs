namespace Framework.Editor.AssetsChecker
{
    using System.Collections.Generic;

    public abstract class CheckerCollection
    {
        public abstract string Name { get; }
        protected List<IRule<CheckerCollection>> rules;
    }
}