using System;
using Framework.UI.Core;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace Framework
{
    public class ViewAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mStart_0 = new CrossBindingMethodInfo("Start");
        static CrossBindingMethodInfo mUpdate_1 = new CrossBindingMethodInfo("Update");
        static CrossBindingMethodInfo mOnShow_3 = new CrossBindingMethodInfo("OnShow");
        static CrossBindingMethodInfo mOnHide_4 = new CrossBindingMethodInfo("OnHide");
        static CrossBindingMethodInfo mOnVmChange_5 = new CrossBindingMethodInfo("OnVmChange");
        static CrossBindingFunctionInfo<Framework.UI.Core.UILevel> mget_UILevel_6 = new CrossBindingFunctionInfo<Framework.UI.Core.UILevel>("get_UILevel");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsSingle_7 = new CrossBindingFunctionInfo<System.Boolean>("get_IsSingle");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Framework.UI.Core.View);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Framework.UI.Core.View, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance => instance;

            protected override void Start()
            {
                if (mStart_0.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_0.Invoke(this.instance);
            }

            protected override void Update()
            {
                if (mUpdate_1.CheckShouldInvokeBase(this.instance))
                    base.Update();
                else
                    mUpdate_1.Invoke(this.instance);
            }

            protected override void OnShow()
            {
                if (mOnShow_3.CheckShouldInvokeBase(this.instance))
                    base.OnShow();
                else
                    mOnShow_3.Invoke(this.instance);
            }

            protected override void OnHide()
            {
                if (mOnHide_4.CheckShouldInvokeBase(this.instance))
                    base.OnHide();
                else
                    mOnHide_4.Invoke(this.instance);
            }

            protected override void OnVmChange()
            {
                mOnVmChange_5.Invoke(this.instance);
            }

            public override Framework.UI.Core.UILevel UILevel
            {
            get
            {
                if (mget_UILevel_6.CheckShouldInvokeBase(this.instance))
                    return base.UILevel;
                else
                    return mget_UILevel_6.Invoke(this.instance);

            }
            }

            public override System.Boolean IsSingle
            {
            get
            {
                if (mget_IsSingle_7.CheckShouldInvokeBase(this.instance))
                    return base.IsSingle;
                else
                    return mget_IsSingle_7.Invoke(this.instance);

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}