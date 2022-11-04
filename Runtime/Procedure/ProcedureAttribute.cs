namespace Framework
{
    public class ProcedureAttribute : ManagerAttribute
    {
        private static int tag = 0;
        public ProcedureAttribute(int manualTag) : base(manualTag)
        {
        }

        public ProcedureAttribute() : base(tag)
        {
            tag++;
        }
    }
}