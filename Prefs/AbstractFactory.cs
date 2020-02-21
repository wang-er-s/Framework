namespace Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractFactory : IFactory
    {
        private IEncryptor _encryptor;
        private ISerializer _serializer;

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
            this._serializer = serializer;
            this._encryptor = encryptor;

            if (this._serializer == null)
                this._serializer = new DefaultSerializer();

            if (this._encryptor == null)
                this._encryptor = new DefaultEncryptor();
        }
        
        public IEncryptor Encryptor
        {
            get { return this._encryptor; }
            protected set { this._encryptor = value; }
        }
        
        public ISerializer Serializer
        {
            get { return this._serializer; }
            protected set { this._serializer = value; }
        }
        
        public abstract Preferences Create(string name);
    }
}
