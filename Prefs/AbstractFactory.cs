namespace Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractFactory : IFactory
    {
        private IEncryptor encryptor;
        private ISerializer serializer;

        public AbstractFactory() : this(null, null)
        {
        }
        
        public AbstractFactory(ISerializer serializer) : this(serializer, null)
        {
        }
        
        public AbstractFactory(ISerializer serializer, IEncryptor encryptor)
        {
#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            this.serializer = serializer;
            this.encryptor = encryptor;

            if (this.serializer == null)
                this.serializer = new DefaultSerializer();

            if (this.encryptor == null)
                this.encryptor = new DefaultEncryptor();
        }
        
        public IEncryptor Encryptor
        {
            get { return this.encryptor; }
            protected set { this.encryptor = value; }
        }
        
        public ISerializer Serializer
        {
            get { return this.serializer; }
            protected set { this.serializer = value; }
        }
        
        public abstract Preferences Create(string name);
    }
}
