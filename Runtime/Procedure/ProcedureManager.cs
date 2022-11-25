using System;

namespace Framework
{
    public class ProcedureManager : GameModuleWithAttribute<ProcedureManager, ProcedureAttribute, int>
    {
        public static ProcedureManager Ins => SingletonProperty<ProcedureManager>.Instance;
        private FsmManager m_FsmManager => FsmManager.Ins;
        private IFsm<ProcedureManager> m_ProcedureFsm;


        public override void Init()
        {
            ProcedureBase[] states = new ProcedureBase[ClassDataMap.Count];
            int index = 0;
            foreach (var value in ClassDataMap.Values)
            {
                states[index] = (ProcedureBase)ReflectionHelper.CreateInstance(value.Type);
                index++;
            }

            m_ProcedureFsm = m_FsmManager.CreateFsm(this, states);
        }

        public void ChangeProcedure(int tag)
        {
            ChangeProcedure(ClassDataMap[tag].Type);
        }

        public void ChangeProcedure<T>() where T : ProcedureBase
        {
            ChangeProcedure(typeof(T));
        }

        public void ChangeProcedure(Type type)
        {
            if (!m_ProcedureFsm.IsRunning)
            {
                m_ProcedureFsm.Start(type);
                return;
            }

            var tmpFsm = (Fsm<ProcedureManager>)m_ProcedureFsm;
            tmpFsm.ChangeState(type);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public T GetProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return m_ProcedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return (ProcedureBase)m_ProcedureFsm.GetState(procedureType);
        }
    }
}