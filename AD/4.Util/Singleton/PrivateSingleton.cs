using System;
using System.Reflection;

namespace AD
{
    public class PrivateSingleton<T>
    {
        private static readonly object locker = new object ();

        public static readonly Type[] EmptyTypes = new Type[ 0 ];

        private static T instance;

        protected static T Ins
        {
            get
            {
                lock ( locker )
                {
                    if ( instance == null )
                    {
                        lock ( locker )
                        {
                            ConstructorInfo ci =
                                typeof ( T ).GetConstructor ( BindingFlags.NonPublic | BindingFlags.Instance, null,
                                    EmptyTypes, null );
                            if ( ci == null )
                            {
                                throw new InvalidOperationException ( "class must contain a private constructor" );
                            }

                            instance = (T) ci.Invoke ( null );
                        }
                    }
                }

                return instance;
            }
        }
    }
}