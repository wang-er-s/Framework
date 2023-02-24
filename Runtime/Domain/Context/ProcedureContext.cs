
using System;

namespace Framework
{
    public class ProcedureContext : Context
    {
        private IRes res;

        public ProcedureContext()
        {
            res = Res.Create();
            Res.SetDefaultRes(res);
        }

        public override void Dispose()
        {
            res.Dispose();
            GC.Collect();
        }
    }
}