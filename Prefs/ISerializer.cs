using System;

namespace Framework.Prefs
{
    public interface ISerializer
    {

        void AddTypeEncoder(ITypeEncoder encoder);

        void RemoveTypeEncoder(ITypeEncoder encoder);

        object Deserialize(string input, Type type);

        string Serialize(object value);
    }
}
