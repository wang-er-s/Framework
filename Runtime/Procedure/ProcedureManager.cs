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

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (m_ProcedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            
            m_ProcedureFsm.Start<T>();
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <param name="procedureType">要开始的流程类型。</param>
        public void StartProcedure(Type procedureType)
        {
            if (m_ProcedureFsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            m_ProcedureFsm.Start(procedureType);
        }

        public void StartProcedure(int tag)
        {
            StartProcedure(ClassDataMap[tag].Type);
        }

        public void ChangeState<T>() where T : ProcedureBase
        {
            var tmpFsm = (Fsm<ProcedureManager>) m_ProcedureFsm;
            tmpFsm.ChangeState<T>();
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