namespace Framework.Editor.AssetsChecker
{
    public interface IRule<T> where T : CheckerCollection
    {
        string Description { get; }

        RulePriority Priority { get; }

        void Run(out bool hasTable, out RuleDataTable table);
    }

    public enum RulePriority
    {
        Medium,
        High,
    }
}