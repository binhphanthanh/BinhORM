using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace SystemFramework.Common.QueryBase
{
    public static class EmitHelper
    {
        private static Hashtable mTypeHash = new Hashtable();
        private static AssemblyBuilder _assembly;
        private static ModuleBuilder _module;

        static EmitHelper()
        {
            mTypeHash[typeof(sbyte)] = OpCodes.Ldind_I1;
            mTypeHash[typeof(byte)] = OpCodes.Ldind_U1;
            mTypeHash[typeof(char)] = OpCodes.Ldind_U2;
            mTypeHash[typeof(short)] = OpCodes.Ldind_I2;
            mTypeHash[typeof(ushort)] = OpCodes.Ldind_U2;
            mTypeHash[typeof(int)] = OpCodes.Ldind_I4;
            mTypeHash[typeof(uint)] = OpCodes.Ldind_U4;
            mTypeHash[typeof(long)] = OpCodes.Ldind_I8;
            mTypeHash[typeof(ulong)] = OpCodes.Ldind_I8;
            mTypeHash[typeof(bool)] = OpCodes.Ldind_I1;
            mTypeHash[typeof(double)] = OpCodes.Ldind_R8;
            mTypeHash[typeof(float)] = OpCodes.Ldind_R4;
        }

        public static ObjectAccessor CreateObjectAccessor(Type mTargetType)
        {
            if(_assembly == null)
            {
                EmitAssembly();
            }
            
            ObjectAccessor objectAccessor = new ObjectAccessor();
            PropertyInfo[] propertyInfos = mTargetType.GetProperties();
            foreach(PropertyInfo property in propertyInfos)
            {
                objectAccessor[property.Name] = EmitPropertyAccessor(mTargetType, property);
            }
            return objectAccessor;
        }

        private static void EmitAssembly()
        {
            // Create an assembly name
            AssemblyName assemblyName = new AssemblyName("PropertyAccessorAssembly");

            // Create a new assembly with one module
            _assembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _module = _assembly.DefineDynamicModule("Module");
        }

        private static TypeBuilder EmitType(string name)
        {
            TypeBuilder type = _module.DefineType(name, TypeAttributes.Public);
            type.AddInterfaceImplementation(typeof(IPropertyAccessor));
            type.DefineDefaultConstructor(MethodAttributes.Public);
            return type;
        }

        /// <summary>
        /// Create an assembly that will provide the get and set methods.
        /// </summary>
        private static PropertyAccessor EmitPropertyAccessor(Type mTargetType, PropertyInfo property)
        {
            string typeName = mTargetType.Name + property.Name;
            TypeBuilder myType = EmitType(typeName);

            EmitGetMethod(myType, mTargetType, property);
            EmitSetMethod(myType, mTargetType, property);

            myType.CreateType();
            IPropertyAccessor accessor = _assembly.CreateInstance(typeName) as IPropertyAccessor;
            return new PropertyAccessor(accessor, property.PropertyType, property.Name, property.CanRead, property.CanWrite);
        }

        private static void EmitGetMethod(TypeBuilder myType, Type mTargetType, PropertyInfo property)
        {
            // Define a method for the get operation. 
            Type[] getParamTypes = new Type[] { typeof(object) };
            Type getReturnType = typeof(object);
            MethodBuilder getMethod =
                myType.DefineMethod("Get",
                MethodAttributes.Public | MethodAttributes.Virtual,
                getReturnType,
                getParamTypes);

            // From the method, get an ILGenerator. This is used to emit the IL that we want.
            ILGenerator getIL = getMethod.GetILGenerator();

            // Emit the IL. 
            MethodInfo targetGetMethod = mTargetType.GetMethod("get_" + property.Name);

            if (targetGetMethod != null)
            {
                getIL.DeclareLocal(typeof(object));
                getIL.Emit(OpCodes.Ldarg_1); //Load the first argument (target object)
                getIL.Emit(OpCodes.Castclass, mTargetType); //Cast to the source type
                getIL.EmitCall(OpCodes.Call, targetGetMethod, null); //Get the property value

                if (targetGetMethod.ReturnType.IsValueType)
                {
                    getIL.Emit(OpCodes.Box, targetGetMethod.ReturnType); //Box if necessary
                }
                getIL.Emit(OpCodes.Stloc_0); //Store it
                getIL.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                getIL.ThrowException(typeof(MissingMethodException));
            }

            getIL.Emit(OpCodes.Ret);
        }

        private static void EmitSetMethod(TypeBuilder myType, Type mTargetType, PropertyInfo property)
        {
            // Define a method for the set operation.
            Type[] setParamTypes = new Type[] { typeof(object), typeof(object) };
            Type setReturnType = null;
            MethodBuilder setMethod =
                myType.DefineMethod("Set",
                MethodAttributes.Public | MethodAttributes.Virtual,
                setReturnType,
                setParamTypes);

            // From the method, get an ILGenerator. This is used to emit the IL that we want.
            ILGenerator setIL = setMethod.GetILGenerator();

            // Emit the IL. 
            MethodInfo targetSetMethod = mTargetType.GetMethod("set_" + property.Name);
            if (targetSetMethod != null)
            {
                Type paramType = targetSetMethod.GetParameters()[0].ParameterType;

                setIL.DeclareLocal(paramType);
                setIL.Emit(OpCodes.Ldarg_1); //Load the first argument (target object)
                setIL.Emit(OpCodes.Castclass, mTargetType);	//Cast to the source type
                setIL.Emit(OpCodes.Ldarg_2); //Load the second argument (value object)
                
                if (paramType.IsValueType)
                {
                    setIL.Emit(OpCodes.Unbox, paramType); //Unbox it 	
                    if (mTypeHash[paramType] != null) //and load
                    {
                        OpCode load = (OpCode)mTypeHash[paramType];
                        setIL.Emit(load);
                    }
                    else
                    {
                        setIL.Emit(OpCodes.Ldobj, paramType);
                    }
                }
                else
                {
                    setIL.Emit(OpCodes.Castclass, paramType); //Cast class
                }
                setIL.EmitCall(OpCodes.Callvirt, targetSetMethod, null);	//Set the property value
            }
            else
            {
                setIL.ThrowException(typeof(MissingMethodException));
            }

            setIL.Emit(OpCodes.Ret);
        }
    }
}
