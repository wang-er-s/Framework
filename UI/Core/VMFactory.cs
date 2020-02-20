namespace Framework.UI.Core
{
    public class VMFactory
    {
        public static T Create<T>() where T : ViewModel,new()
        {
            T vm = new T();
            return vm;
        } 
    }
}