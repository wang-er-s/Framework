using System;
using System.Collections.Generic;

namespace AD
{
    public class EventDispatcher
    {
        private Dictionary<Type, IRegisters> typeEventDic = new Dictionary<Type, IRegisters>();
        private Dictionary<int, List<Action>> enumEventDic = new Dictionary<int, List<Action>>();

        public void Register<T>(Action<T> listener)
        {
            if (listener == null)
            {
                Log.Error("AddListener: listener不能为空");
                return;
            }
            if (typeEventDic.TryGetValue(typeof(T), out var iRegisters))
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
                Log.Error("RemoveListener: listener不能为空");
                return;
            }
            if (!typeEventDic.TryGetValue(typeof(T), out var iRegisters)) return;
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
            if (typeEventDic.TryGetValue(typeof(T), out var iRegisters))
            {
                try
                {
                    var registers = iRegisters as Registers<T>;
                    registers.ForEach(listener => listener.Invoke(msg));
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"SendMessage:{typeof(T)} {e.Message} {e.StackTrace}");
                }
            }
            Log.Warning($"{typeof(T)} must register first!");
        }

        public void Register(int tag, Action listener)
        {
            if (listener == null)
            {
                Log.Error("AddListener: listener不能为空");
                return;
            }
            if (enumEventDic.TryGetValue(tag, out var registers))
            {
                registers.Add(listener);
            }
            else
            {
                var newRegisters = new List<Action> {listener};
                enumEventDic.Add(tag, newRegisters);
            }
        }

        public void UnRegister(int tag, Action listener)
        {
            if (listener == null)
            {
                Log.Error("RemoveListener: listener不能为空");
                return;
            }
            if (!enumEventDic.TryGetValue(tag, out var registers)) return;
            registers.Remove(listener);
        }

        public void SendMessage(int tag)
        {
            if (enumEventDic.TryGetValue(tag, out var registers))
            {
                try
                {
                    registers.ForEach(listener => listener.Invoke());
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"SendMessage: tag={tag} {e.Message} {e.StackTrace}");
                }
            }
            Log.Warning($"tag={tag} must register first!");
        }
    }
}