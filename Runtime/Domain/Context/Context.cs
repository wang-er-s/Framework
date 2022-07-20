/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System.Collections.Generic;

namespace Framework.Contexts
{
    public class Context
    {
        private static ApplicationContext context = new ApplicationContext();
        private static Dictionary<string, Context> contexts = new Dictionary<string, Context>();
        public static ApplicationContext GetApplicationContext()
        {
            return Context.context;
        }

        public static void SetApplicationContext(ApplicationContext context)
        {
            Context.context = context;
        }
        
        public static Context GetContext(string key)
        {
            Context context = null;
            contexts.TryGetValue(key, out context);
            return context;
        }

        public static T GetContext<T>(string key) where T : Context
        {
            return (T)GetContext(key);
        }

        public static T GetContext<T>() where T : Context
        {
            return (T) GetContext(typeof(T).Name);
        }

        public static void AddContext<T>(T context) where T : Context
        {
            AddContext(context.GetCLRType().Name, context);
        }

        public static void AddContext(string key, Context context)
        {
            contexts.Add(key, context);
        }

        public static void RemoveContext<T>()
        {
            RemoveContext(typeof(T).Name);
        }

        public static void RemoveContext(string key)
        {
            if (contexts.ContainsKey(key))
                contexts.Remove(key);
        }
        
        private bool _innerContainer;
        private Context _contextBase;
        private Dictionary<string, object> _attributes;

        public Context() : this(null)
        {
        }

        public Context(Context contextBase)
        {
            _attributes = new Dictionary<string, object>();
            _contextBase = contextBase;
        }
        
        public virtual bool Contains(string name, bool cascade = true)
        {
            if (this._attributes.ContainsKey(name))
                return true;

            if (cascade && this._contextBase != null)
                return this._contextBase.Contains(name, cascade);

            return false;
        }
        
        public virtual bool Contains<T>( bool cascade = true)
        {
            return Contains(typeof(T).Name, cascade);
        }

        public virtual object Get(string name, bool cascade = true)
        {
            return this.Get<object>(name, cascade);
        }

        public virtual T Get<T>(bool cascade = true)
        {
            return Get<T>(typeof(T).Name, cascade);
        }
        
        public virtual T Get<T>(string name, bool cascade = true)
        {
            object v;
            if (this._attributes.TryGetValue(name, out v))
                return (T)v;

            if (cascade && this._contextBase != null)
                return this._contextBase.Get<T>(name, cascade);

            return default(T);
        }

        public virtual void Set(string name, object value)
        {
            this._attributes[name] = value;
        }

        public virtual void Set<T>(T value)
        {
            Set(typeof(T).Name, value);
        }

        public virtual object Remove(string name)
        {
            return this.Remove<object>(name);
        }

        public virtual T Remove<T>()
        {
            return Remove<T>(typeof(T).Name);
        }

        public virtual T Remove<T>(string name)
        {
            if (!this._attributes.ContainsKey(name))
                return default(T);

            object v = this._attributes[name];
            this._attributes.Remove(name);
            return (T)v;
        }
    }
}