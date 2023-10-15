namespace Framework
{
    public abstract class ETObject
    {
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}