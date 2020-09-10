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

namespace Framework.Prefs
{

    public abstract class AbstractFactory : IFactory
    {
        private IEncryptor _encryptor;
        private ISerializer _serializer;

        public AbstractFactory() : this(null, null)
        {
        }
        
        public AbstractFactory(ISerializer serializer) : this(serializer, null)
        {
        }
        
        public AbstractFactory(ISerializer serializer, IEncryptor encryptor)
        {
#if UNITY_IOS
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            this._serializer = serializer;
            this._encryptor = encryptor;

            if (this._serializer == null)
                this._serializer = new DefaultSerializer();

            if (this._encryptor == null)
                this._encryptor = new DefaultEncryptor();
        }
        
        public IEncryptor Encryptor
        {
            get => this._encryptor;
            protected set => this._encryptor = value;
        }
        
        public ISerializer Serializer
        {
            get => this._serializer;
            protected set => this._serializer = value;
        }
        
        public abstract Preferences Create(string name);
    }
}
