    using Framework;

    public class ConfigAttribute : ManagerAttribute
    {
        public string Path { get; private set; }
        
        public ConfigAttribute(string path) : base(0)
        {
            Path = path;
        }

        public override string IndexName { get; } = nameof(Path);
    }
