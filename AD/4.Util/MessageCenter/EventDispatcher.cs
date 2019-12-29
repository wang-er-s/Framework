using System;
using System.Collections.Generic;

namespace AD
{
    public class EventDispatcher
    {
        private Dictionary<Type, IRegisters> typeEventDic = new Dictionary<Type, IRegisters>();
        private Dictionary<string, List<Action>> enumEventDic = new Dictionary<string, List<Action>>();

        public void Register<T>(Action<T> listener)
        {
            if (listener == null)
            {
                Debugger.Error("AddListener: listener不能为空");
                return;
            }
            IRegisters iRegisters;
            if (typeEventDic.TryGetValue(typeof(T), out iRegisters))
            {
                Registers<T> registers = iRegisters as Registers<T>;
                registers.Add(listener);
            }
            else
            {
                Registers<T> registers = new Registers<T>();
                registers.Add(listener);
                typeEventDic.Add(typeof(T), registers);
            }
        }

        public void UnRegister<T>(Action<T> listener)
        {
            if (listener == null)
            {
                Debugger.Error("RemoveListener: listener不能为空");
                return;
            }
            IRegisters iRegisters;
            if (!typeEventDic.TryGetValue(typeof(T), out iRegisters)) return;
            Registers<T> registers = iRegisters as Registers<T>;
            registers.Remove(listener);
        }

        public void Clear()
        {
            foreach (var iRegisters in typeEventDic.Values)
            {
                var registers = iRegisters as Registers<Type>;
                registers.Clear();
            }
            foreach (var registers in enumEventDic.Values)
            {
                registers.Clear();
            }
            typeEventDic.Clear();
            enumEventDic.Clear();
        }

        public void SendMessage<T>(T msg)
        {
            IRegisters iRegisters;
            if (typeEventDic.TryGetValue(typeof(T), out iRegisters))
            {
                try
                {
                    var registers = iRegisters as Registers<T>;
                    registers.ForEach(listener => listener.Invoke(msg));
                    return;
                }
                catch (Exception e)
                {
                    Debugger.Error($"SendMessage:{typeof(T)} {e.Message} {e.StackTrace}");
                }
            }
            Debugger.Warning($"{typeof(T)} must register first!");
        }

        public void Register(string tag, Action listener)
        {
            if (listener == null)
            {
                Debugger.Error("AddListener: listener不能为空");
                return;
            }
            List<Action> registers;
            if (enumEventDic.TryGetValue(tag, out registers))
            {
                registers.Add(listener);
            }
            else
            {
                var newRegisters = new List<Action> {listener};
                enumEventDic.Add(tag, newRegisters);
            }
        }

        public void UnRegister(string tag, Action listener)
        {
            if (listener == null)
            {
                Debugger.Error("RemoveListener: listener不能为空");
                return;
            }
            List<Action> registers;
            if (!enumEventDic.TryGetValue(tag, out registers)) return;
            registers.Remove(listener);
        }

        public void SendMessage(string tag)
        {
            List<Action> registers;
            if (enumEventDic.TryGetValue(tag, out registers))
            {
                try
                {
                    registers.ForEach(listener => listener.Invoke());
                    return;
                }
                catch (Exception e)
                {
                    Debugger.Error($"SendMessage: tag={tag} {e.Message} {e.StackTrace}");
                }
            }
            Debugger.Warning($"tag={tag} must register first!");
        }
    }
}