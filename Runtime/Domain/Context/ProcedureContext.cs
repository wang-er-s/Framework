
namespace Framework
{
    public class ProcedureContext : Context
    {
        public IRes Res { get; }

        public ProcedureContext()
        {
            Res = Framework.Res.Create();
            Framework.Res.SetDefaultRes(Res);
        }

        public override void Dispose()
        {
            Res.Release();
        }
    }
}