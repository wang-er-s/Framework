using System.Collections;
using System.Collections.Generic;
using Framework;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void 试一试()
        {
            var call = Substitute.For<EventListener<BMsg>>();
            
        }
    }

    public interface Call<T> : EventListenerBase
    {
        void Add();
    }
}
