
namespace Framework
{
    public class DomainContext : Context
    {
        public IRes Res { get; }

        public DomainContext()
        {
            Res = Framework.Res.Create();
        }
    }
}