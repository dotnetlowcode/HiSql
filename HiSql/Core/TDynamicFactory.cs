using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HiSql
{
    /// <summary>
    /// 反射动态赋值委托
    /// </summary>
    /// <param name="target"></param>
    /// <param name="arg"></param>
    //public delegate void SetValueDelegate(object target, object arg);
    public   class TDynamicFactory
    {
        public delegate void SetValueDelegate(object target, object arg);

        public SetValueDelegate hander;
        private TDynamicFactory DynamicBuilder;

        public SetValueDelegate CreatePropertySetter(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!property.CanWrite)
                return null;

            MethodInfo setMethod = property.GetSetMethod(true);
            
            DynamicMethod dm = new DynamicMethod("PropertySetter", null,
                new Type[] { typeof(object), typeof(object) }, property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if (!setMethod.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            if (!setMethod.IsStatic && !property.DeclaringType.IsValueType)
            {
                //调用的是非静态方法
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, setMethod, null);

            il.Emit(OpCodes.Ret);

            //DynamicBuilder.hander= (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate)); ;
        }

        private   void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }
    }
}
