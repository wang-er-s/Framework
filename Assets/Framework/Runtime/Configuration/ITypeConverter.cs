using System;

namespace Framework.Configuration
{
    public interface ITypeConverter
    {
        bool Support(Type type);

        object Convert(Type type, object value);
    }
}